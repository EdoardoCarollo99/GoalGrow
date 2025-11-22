using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Services.Interfaces;
using GoalGrow.Data;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.API.Services.Implementations
{
    /// <summary>
    /// Service for managing savings goals
    /// </summary>
    public class GoalService : IGoalService
    {
        private readonly GoalGrowDbContext _context;
        private readonly ILogger<GoalService> _logger;

        public GoalService(GoalGrowDbContext context, ILogger<GoalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Get Goals

        /// <summary>
        /// Gets all goals for the current user with pagination and filters
        /// </summary>
        public async Task<GoalListResponse> GetUserGoalsAsync(
            Guid userId,
            int pageNumber = 1,
            int pageSize = 20,
            string? status = null,
            string? type = null)
        {
            var query = _context.Goals
                .Where(g => g.UserId == userId);

            // Apply status filter
            if (!string.IsNullOrWhiteSpace(status))
            {
                if (Enum.TryParse<GoalStatus>(status, ignoreCase: true, out var parsedStatus))
                {
                    query = query.Where(g => g.Status == parsedStatus);
                }
            }

            // Apply type filter
            if (!string.IsNullOrWhiteSpace(type))
            {
                if (Enum.TryParse<GoalType>(type, ignoreCase: true, out var parsedType))
                {
                    query = query.Where(g => g.Type == parsedType);
                }
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var goals = await query
                .OrderByDescending(g => g.Priority)
                .ThenByDescending(g => g.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Calculate summary stats
            var allUserGoals = await _context.Goals
                .Where(g => g.UserId == userId)
                .ToListAsync();

            var totalSaved = allUserGoals.Sum(g => g.CurrentAmount);
            var totalTarget = allUserGoals.Sum(g => g.TargetAmount);
            var activeCount = allUserGoals.Count(g => g.Status == GoalStatus.Active);
            var completedCount = allUserGoals.Count(g => g.Status == GoalStatus.Completed);

            var goalSummaries = goals.Select(MapToGoalSummary).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new GoalListResponse
            {
                Goals = goalSummaries,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages,
                TotalSaved = totalSaved,
                TotalTarget = totalTarget,
                ActiveGoalsCount = activeCount,
                CompletedGoalsCount = completedCount
            };
        }

        /// <summary>
        /// Gets a specific goal by ID
        /// </summary>
        public async Task<GoalResponse?> GetGoalByIdAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);

            if (goal == null)
            {
                _logger.LogWarning("Goal not found: {GoalId} for user {UserId}", goalId, userId);
                return null;
            }

            return MapToGoalResponse(goal);
        }

        #endregion

        #region Create/Update/Delete Goals

        /// <summary>
        /// Creates a new savings goal
        /// </summary>
        public async Task<GoalResponse> CreateGoalAsync(Guid userId, CreateGoalRequest request)
        {
            // Validate target date is in the future
            if (request.TargetDate <= DateTime.UtcNow)
            {
                throw new ArgumentException("Target date must be in the future");
            }

            // Parse enums
            if (!Enum.TryParse<GoalType>(request.Type, ignoreCase: true, out var goalType))
            {
                goalType = GoalType.Custom;
            }

            if (!Enum.TryParse<GoalPriority>(request.Priority, ignoreCase: true, out var priority))
            {
                priority = GoalPriority.Medium;
            }

            RecurrenceFrequency? autoSaveFreq = null;
            if (!string.IsNullOrWhiteSpace(request.AutoSaveFrequency))
            {
                Enum.TryParse<RecurrenceFrequency>(request.AutoSaveFrequency, ignoreCase: true, out var freq);
                autoSaveFreq = freq;
            }

            // Create goal entity
            var goal = new Goal(
                name: request.Name,
                targetAmount: request.TargetAmount,
                targetDate: request.TargetDate,
                userId: userId,
                type: goalType)
            {
                Description = request.Description ?? string.Empty,
                Priority = priority,
                Category = request.Category ?? string.Empty,
                IconUrl = request.IconUrl ?? string.Empty,
                IsAutoSave = request.EnableAutoSave,
                AutoSaveAmount = request.AutoSaveAmount ?? 0,
                AutoSaveFrequency = autoSaveFreq,
                IsWithdrawalLocked = request.LockWithdrawals,
                CurrentAmount = 0,
                Status = GoalStatus.Active
            };

            _context.Goals.Add(goal);

            // If initial amount provided, create a contribution
            if (request.InitialAmount.HasValue && request.InitialAmount.Value > 0)
            {
                // Get investor user to check wallet balance
                var user = await _context.Users.OfType<InversotorUser>()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found or not an investor");
                }

                if (user.VirtualWalletBalance < request.InitialAmount.Value)
                {
                    throw new InvalidOperationException("Insufficient funds in virtual wallet");
                }

                // Deduct from wallet
                user.VirtualWalletBalance -= request.InitialAmount.Value;

                // Add to goal
                goal.CurrentAmount = request.InitialAmount.Value;

                _logger.LogInformation("Initial contribution of {Amount} made to goal {GoalId}", 
                    request.InitialAmount.Value, goal.Id);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal created: {GoalId} for user {UserId}", goal.Id, userId);

            return MapToGoalResponse(goal);
        }

        /// <summary>
        /// Updates an existing goal
        /// </summary>
        public async Task<GoalResponse> UpdateGoalAsync(Guid goalId, Guid userId, UpdateGoalRequest request)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            // Prevent editing system goals
            if (goal.IsSystemGoal)
            {
                throw new InvalidOperationException("Cannot edit system goals");
            }

            // Update properties if provided
            if (!string.IsNullOrWhiteSpace(request.Name))
                goal.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                goal.Description = request.Description;

            if (request.TargetAmount.HasValue)
            {
                if (request.TargetAmount.Value < goal.CurrentAmount)
                {
                    throw new ArgumentException("Target amount cannot be less than current amount");
                }
                goal.TargetAmount = request.TargetAmount.Value;
            }

            if (request.TargetDate.HasValue)
            {
                if (request.TargetDate.Value <= DateTime.UtcNow)
                {
                    throw new ArgumentException("Target date must be in the future");
                }
                goal.TargetDate = request.TargetDate.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Priority))
            {
                if (Enum.TryParse<GoalPriority>(request.Priority, ignoreCase: true, out var priority))
                {
                    goal.Priority = priority;
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
                goal.Category = request.Category;

            if (!string.IsNullOrWhiteSpace(request.IconUrl))
                goal.IconUrl = request.IconUrl;

            if (request.EnableAutoSave.HasValue)
                goal.IsAutoSave = request.EnableAutoSave.Value;

            if (request.AutoSaveAmount.HasValue)
                goal.AutoSaveAmount = request.AutoSaveAmount.Value;

            if (!string.IsNullOrWhiteSpace(request.AutoSaveFrequency))
            {
                if (Enum.TryParse<RecurrenceFrequency>(request.AutoSaveFrequency, ignoreCase: true, out var freq))
                {
                    goal.AutoSaveFrequency = freq;
                }
            }

            if (request.LockWithdrawals.HasValue)
                goal.IsWithdrawalLocked = request.LockWithdrawals.Value;

            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal updated: {GoalId}", goalId);

            return MapToGoalResponse(goal);
        }

        /// <summary>
        /// Deletes a goal (changes status to Cancelled)
        /// </summary>
        public async Task<bool> DeleteGoalAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            // Prevent deleting system goals
            if (goal.IsSystemGoal)
            {
                throw new InvalidOperationException("Cannot delete system goals");
            }

            // If goal has funds, return them to wallet
            if (goal.CurrentAmount > 0)
            {
                var user = await _context.Users.OfType<InversotorUser>()
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user != null)
                {
                    user.VirtualWalletBalance += goal.CurrentAmount;
                    _logger.LogInformation("Returned {Amount} to wallet from deleted goal {GoalId}", 
                        goal.CurrentAmount, goalId);
                }
            }

            // Soft delete - mark as cancelled
            goal.Status = GoalStatus.Cancelled;
            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal deleted (cancelled): {GoalId}", goalId);

            return true;
        }

        #endregion

        #region Goal Transactions (Contribute/Withdraw)

        /// <summary>
        /// Contributes funds to a goal from virtual wallet
        /// </summary>
        public async Task<GoalResponse> ContributeToGoalAsync(Guid goalId, Guid userId, GoalContributionRequest request)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            // Check goal status
            if (goal.Status == GoalStatus.Completed)
            {
                throw new InvalidOperationException("Cannot contribute to a completed goal");
            }

            if (goal.Status == GoalStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot contribute to a cancelled goal");
            }

            // Get user wallet
            var user = await _context.Users.OfType<InversotorUser>()
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException("User not found or not an investor");

            // Check wallet balance
            if (user.VirtualWalletBalance < request.Amount)
            {
                throw new InvalidOperationException($"Insufficient funds. Available: €{user.VirtualWalletBalance:F2}");
            }

            // Transfer funds
            user.VirtualWalletBalance -= request.Amount;
            goal.CurrentAmount += request.Amount;

            // Check if goal is now completed
            if (goal.CurrentAmount >= goal.TargetAmount && goal.Status == GoalStatus.Active)
            {
                goal.Status = GoalStatus.Completed;
                goal.CompletedAt = DateTime.UtcNow;
                _logger.LogInformation("Goal {GoalId} completed!", goalId);
            }

            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Contributed {Amount} to goal {GoalId}", request.Amount, goalId);

            return MapToGoalResponse(goal);
        }

        /// <summary>
        /// Withdraws funds from a goal back to virtual wallet
        /// </summary>
        public async Task<GoalResponse> WithdrawFromGoalAsync(Guid goalId, Guid userId, GoalWithdrawalRequest request)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            // Check if withdrawals are locked
            if (goal.IsWithdrawalLocked)
            {
                throw new InvalidOperationException("Withdrawals are locked for this goal");
            }

            // Check goal status
            if (goal.Status == GoalStatus.Cancelled)
            {
                throw new InvalidOperationException("Cannot withdraw from a cancelled goal");
            }

            // Check if sufficient funds in goal
            if (goal.CurrentAmount < request.Amount)
            {
                throw new InvalidOperationException($"Insufficient funds in goal. Available: €{goal.CurrentAmount:F2}");
            }

            // Get user wallet
            var user = await _context.Users.OfType<InversotorUser>()
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new InvalidOperationException("User not found or not an investor");

            // Transfer funds back
            goal.CurrentAmount -= request.Amount;
            user.VirtualWalletBalance += request.Amount;

            // If goal was completed, revert to active
            if (goal.Status == GoalStatus.Completed)
            {
                goal.Status = GoalStatus.Active;
                goal.CompletedAt = null;
                _logger.LogInformation("Goal {GoalId} reverted to active after withdrawal", goalId);
            }

            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Withdrawn {Amount} from goal {GoalId}. Reason: {Reason}", 
                request.Amount, goalId, request.Reason);

            return MapToGoalResponse(goal);
        }

        #endregion

        #region Goal Progress & Status

        /// <summary>
        /// Gets detailed progress for a goal
        /// </summary>
        public async Task<GoalProgressResponse> GetGoalProgressAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            var totalDays = (goal.TargetDate - goal.CreatedAt).Days;
            var daysElapsed = (DateTime.UtcNow - goal.CreatedAt).Days;
            var daysRemaining = Math.Max(0, (goal.TargetDate - DateTime.UtcNow).Days);

            var daysElapsedPercentage = totalDays > 0 ? (daysElapsed / (double)totalDays) * 100 : 0;

            // Calculate recommended savings
            var remainingAmount = Math.Max(0, goal.TargetAmount - goal.CurrentAmount);
            var recommendedDaily = daysRemaining > 0 ? remainingAmount / daysRemaining : 0;
            var recommendedWeekly = daysRemaining > 0 ? (remainingAmount / daysRemaining) * 7 : 0;
            var recommendedMonthly = daysRemaining > 0 ? (remainingAmount / daysRemaining) * 30 : 0;

            // Calculate milestones
            var milestones = new List<MilestoneStatus>
            {
                CreateMilestone("25% Goal", 25, goal),
                CreateMilestone("50% Halfway There!", 50, goal),
                CreateMilestone("75% Almost Done", 75, goal),
                CreateMilestone("100% Goal Achieved!", 100, goal)
            };

            // Performance check
            var expectedProgress = daysElapsedPercentage;
            var actualProgress = (double)goal.ProgressPercentage;
            var isOnTrack = actualProgress >= expectedProgress || goal.Status == GoalStatus.Completed;

            var performanceMessage = goal.Status == GoalStatus.Completed
                ? "?? Congratulations! Goal completed!"
                : isOnTrack
                    ? "? You're on track to reach your goal!"
                    : $"?? You're {expectedProgress - actualProgress:F1}% behind schedule. Consider increasing contributions.";

            return new GoalProgressResponse
            {
                GoalId = goal.Id,
                GoalName = goal.Name,
                CurrentAmount = goal.CurrentAmount,
                TargetAmount = goal.TargetAmount,
                ProgressPercentage = goal.ProgressPercentage,
                RemainingAmount = goal.RemainingAmount,
                CreatedAt = goal.CreatedAt,
                TargetDate = goal.TargetDate,
                DaysRemaining = daysRemaining,
                TotalDays = totalDays,
                DaysElapsedPercentage = (decimal)daysElapsedPercentage,
                RecommendedDailySaving = recommendedDaily,
                RecommendedWeeklySaving = recommendedWeekly,
                RecommendedMonthlySaving = recommendedMonthly,
                Milestones = milestones,
                RecentContributions = new List<GoalTransactionSummary>(), // TODO: Implement when transactions are linked
                IsOnTrack = isOnTrack,
                PerformanceMessage = performanceMessage
            };
        }

        /// <summary>
        /// Marks a goal as completed (manual completion)
        /// </summary>
        public async Task<GoalResponse> CompleteGoalAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            if (goal.Status == GoalStatus.Completed)
            {
                throw new InvalidOperationException("Goal is already completed");
            }

            goal.Status = GoalStatus.Completed;
            goal.CompletedAt = DateTime.UtcNow;
            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal manually completed: {GoalId}", goalId);

            return MapToGoalResponse(goal);
        }

        /// <summary>
        /// Pauses a goal (stops auto-save)
        /// </summary>
        public async Task<GoalResponse> PauseGoalAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            if (goal.Status != GoalStatus.Active)
            {
                throw new InvalidOperationException("Can only pause active goals");
            }

            goal.Status = GoalStatus.OnHold;
            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal paused: {GoalId}", goalId);

            return MapToGoalResponse(goal);
        }

        /// <summary>
        /// Resumes a paused goal
        /// </summary>
        public async Task<GoalResponse> ResumeGoalAsync(Guid goalId, Guid userId)
        {
            var goal = await _context.Goals
                .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId)
                ?? throw new KeyNotFoundException($"Goal with ID {goalId} not found");

            if (goal.Status != GoalStatus.OnHold)
            {
                throw new InvalidOperationException("Can only resume paused goals");
            }

            goal.Status = GoalStatus.Active;
            goal.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Goal resumed: {GoalId}", goalId);

            return MapToGoalResponse(goal);
        }

        #endregion

        #region Admin Statistics

        /// <summary>
        /// Gets platform-wide goal statistics (Admin only)
        /// </summary>
        public async Task<GoalStatsResponse> GetGoalStatsAsync()
        {
            var allGoals = await _context.Goals.ToListAsync();

            var totalGoals = allGoals.Count;
            var activeGoals = allGoals.Count(g => g.Status == GoalStatus.Active);
            var completedGoals = allGoals.Count(g => g.Status == GoalStatus.Completed);
            var pausedGoals = allGoals.Count(g => g.Status == GoalStatus.OnHold);
            var cancelledGoals = allGoals.Count(g => g.Status == GoalStatus.Cancelled);

            var emergencyGoals = allGoals.Count(g => g.Type == GoalType.Emergency);
            var investmentGoals = allGoals.Count(g => g.Type == GoalType.Investment);
            var customGoals = allGoals.Count(g => g.Type == GoalType.Custom);

            var totalAmountSaved = allGoals.Sum(g => g.CurrentAmount);
            var totalTargetAmount = allGoals.Sum(g => g.TargetAmount);
            var averageGoalSize = totalGoals > 0 ? totalTargetAmount / totalGoals : 0;
            var averageCompletionRate = totalGoals > 0 
                ? allGoals.Average(g => g.ProgressPercentage) 
                : 0;

            var thisMonth = DateTime.UtcNow.AddDays(-30);
            var goalsCompletedThisMonth = allGoals.Count(g => 
                g.CompletedAt.HasValue && g.CompletedAt.Value >= thisMonth);

            // TODO: Calculate total contributions this month from transactions
            var totalContributionsThisMonth = 0m;

            return new GoalStatsResponse
            {
                TotalGoals = totalGoals,
                ActiveGoals = activeGoals,
                CompletedGoals = completedGoals,
                PausedGoals = pausedGoals,
                CancelledGoals = cancelledGoals,
                EmergencyGoals = emergencyGoals,
                InvestmentGoals = investmentGoals,
                CustomGoals = customGoals,
                TotalAmountSaved = totalAmountSaved,
                TotalTargetAmount = totalTargetAmount,
                AverageGoalSize = averageGoalSize,
                AverageCompletionRate = averageCompletionRate,
                GoalsCompletedThisMonth = goalsCompletedThisMonth,
                TotalContributionsThisMonth = totalContributionsThisMonth,
                LastUpdated = DateTime.UtcNow
            };
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Maps Goal entity to GoalResponse DTO
        /// </summary>
        private GoalResponse MapToGoalResponse(Goal goal)
        {
            var daysRemaining = Math.Max(0, (goal.TargetDate - DateTime.UtcNow).Days);
            var monthsRemaining = Math.Max(1, daysRemaining / 30);
            var recommendedMonthlySaving = goal.RemainingAmount / monthsRemaining;

            return new GoalResponse
            {
                Id = goal.Id,
                UserId = goal.UserId,
                Name = goal.Name,
                Description = goal.Description,
                Type = goal.Type.ToString(),
                IsSystemGoal = goal.IsSystemGoal,
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                RemainingAmount = goal.RemainingAmount,
                ProgressPercentage = goal.ProgressPercentage,
                Currency = goal.Currency,
                CreatedAt = goal.CreatedAt,
                TargetDate = goal.TargetDate,
                CompletedAt = goal.CompletedAt,
                DaysRemaining = daysRemaining,
                Status = goal.Status.ToString(),
                Priority = goal.Priority.ToString(),
                Category = goal.Category,
                IconUrl = goal.IconUrl,
                IsAutoSave = goal.IsAutoSave,
                AutoSaveAmount = goal.AutoSaveAmount > 0 ? goal.AutoSaveAmount : null,
                AutoSaveFrequency = goal.AutoSaveFrequency?.ToString(),
                IsWithdrawalLocked = goal.IsWithdrawalLocked,
                UnlockThreshold = goal.UnlockThreshold,
                HasReachedUnlockThreshold = goal.HasReachedUnlockThreshold,
                IsCompleted = goal.IsCompleted,
                IsOverdue = goal.TargetDate < DateTime.UtcNow && !goal.IsCompleted,
                RecommendedMonthlySaving = recommendedMonthlySaving
            };
        }

        /// <summary>
        /// Maps Goal entity to GoalSummaryResponse DTO
        /// </summary>
        private GoalSummaryResponse MapToGoalSummary(Goal goal)
        {
            return new GoalSummaryResponse
            {
                Id = goal.Id,
                Name = goal.Name,
                Type = goal.Type.ToString(),
                TargetAmount = goal.TargetAmount,
                CurrentAmount = goal.CurrentAmount,
                ProgressPercentage = goal.ProgressPercentage,
                TargetDate = goal.TargetDate,
                DaysRemaining = Math.Max(0, goal.DaysRemaining),
                Status = goal.Status.ToString(),
                Priority = goal.Priority.ToString(),
                IconUrl = goal.IconUrl,
                IsCompleted = goal.IsCompleted
            };
        }

        /// <summary>
        /// Creates a milestone status
        /// </summary>
        private MilestoneStatus CreateMilestone(string name, decimal percentage, Goal goal)
        {
            var targetAmount = (goal.TargetAmount * percentage) / 100;
            var isReached = goal.CurrentAmount >= targetAmount;

            return new MilestoneStatus
            {
                Name = name,
                Percentage = percentage,
                TargetAmount = targetAmount,
                IsReached = isReached,
                ReachedAt = isReached ? (DateTime?)DateTime.UtcNow : null // TODO: Track actual reach date
            };
        }

        #endregion
    }
}
