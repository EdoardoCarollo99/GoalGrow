using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.Entity.Models;

namespace GoalGrow.API.Services.Interfaces
{
    /// <summary>
    /// Service for managing savings goals
    /// </summary>
    public interface IGoalService
    {
        /// <summary>
        /// Gets all goals for the current user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="status">Filter by status (optional)</param>
        /// <param name="type">Filter by type (optional)</param>
        /// <returns>Paginated list of goals</returns>
        Task<GoalListResponse> GetUserGoalsAsync(Guid userId, int pageNumber = 1, int pageSize = 20, string? status = null, string? type = null);

        /// <summary>
        /// Gets a specific goal by ID
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID (for authorization)</param>
        /// <returns>Goal details or null if not found</returns>
        Task<GoalResponse?> GetGoalByIdAsync(Guid goalId, Guid userId);

        /// <summary>
        /// Creates a new savings goal
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Goal creation data</param>
        /// <returns>Created goal</returns>
        Task<GoalResponse> CreateGoalAsync(Guid userId, CreateGoalRequest request);

        /// <summary>
        /// Updates an existing goal
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID (for authorization)</param>
        /// <param name="request">Updated goal data</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> UpdateGoalAsync(Guid goalId, Guid userId, UpdateGoalRequest request);

        /// <summary>
        /// Deletes a goal (soft delete)
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID (for authorization)</param>
        /// <returns>True if deleted successfully</returns>
        Task<bool> DeleteGoalAsync(Guid goalId, Guid userId);

        /// <summary>
        /// Gets detailed progress for a goal
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID (for authorization)</param>
        /// <returns>Progress details with milestones</returns>
        Task<GoalProgressResponse> GetGoalProgressAsync(Guid goalId, Guid userId);

        /// <summary>
        /// Contributes funds to a goal from virtual wallet
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="request">Contribution data</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> ContributeToGoalAsync(Guid goalId, Guid userId, GoalContributionRequest request);

        /// <summary>
        /// Withdraws funds from a goal back to virtual wallet
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID</param>
        /// <param name="request">Withdrawal data</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> WithdrawFromGoalAsync(Guid goalId, Guid userId, GoalWithdrawalRequest request);

        /// <summary>
        /// Gets platform-wide goal statistics (Admin only)
        /// </summary>
        /// <returns>Goal statistics</returns>
        Task<GoalStatsResponse> GetGoalStatsAsync();

        /// <summary>
        /// Marks a goal as completed (manual completion)
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> CompleteGoalAsync(Guid goalId, Guid userId);

        /// <summary>
        /// Pauses a goal (stops auto-save)
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> PauseGoalAsync(Guid goalId, Guid userId);

        /// <summary>
        /// Resumes a paused goal
        /// </summary>
        /// <param name="goalId">Goal ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>Updated goal</returns>
        Task<GoalResponse> ResumeGoalAsync(Guid goalId, Guid userId);
    }
}
