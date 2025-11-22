namespace GoalGrow.API.DTOs.Responses
{
    /// <summary>
    /// Detailed goal information
    /// </summary>
    public class GoalResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsSystemGoal { get; set; }
        
        // Financial Data
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public string Currency { get; set; } = "EUR";
        
        // Dates
        public DateTime CreatedAt { get; set; }
        public DateTime TargetDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int DaysRemaining { get; set; }
        
        // Status & Priority
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? IconUrl { get; set; }
        
        // Auto-save Configuration
        public bool IsAutoSave { get; set; }
        public decimal? AutoSaveAmount { get; set; }
        public string? AutoSaveFrequency { get; set; }
        
        // Settings
        public bool IsWithdrawalLocked { get; set; }
        public decimal? UnlockThreshold { get; set; }
        public bool HasReachedUnlockThreshold { get; set; }
        
        // Computed Fields
        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public decimal RecommendedMonthlySaving { get; set; }
    }

    /// <summary>
    /// Compact goal summary for lists
    /// </summary>
    public class GoalSummaryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime TargetDate { get; set; }
        public int DaysRemaining { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Goal progress with milestones
    /// </summary>
    public class GoalProgressResponse
    {
        public Guid GoalId { get; set; }
        public string GoalName { get; set; } = string.Empty;
        public decimal CurrentAmount { get; set; }
        public decimal TargetAmount { get; set; }
        public decimal ProgressPercentage { get; set; }
        public decimal RemainingAmount { get; set; }
        
        // Timeline
        public DateTime CreatedAt { get; set; }
        public DateTime TargetDate { get; set; }
        public int DaysRemaining { get; set; }
        public int TotalDays { get; set; }
        public decimal DaysElapsedPercentage { get; set; }
        
        // Recommendations
        public decimal RecommendedDailySaving { get; set; }
        public decimal RecommendedWeeklySaving { get; set; }
        public decimal RecommendedMonthlySaving { get; set; }
        
        // Milestones
        public List<MilestoneStatus> Milestones { get; set; } = new();
        
        // Recent Activity
        public List<GoalTransactionSummary> RecentContributions { get; set; } = new();
        
        // Performance
        public bool IsOnTrack { get; set; }
        public string PerformanceMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Milestone status (25%, 50%, 75%, 100%)
    /// </summary>
    public class MilestoneStatus
    {
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public decimal TargetAmount { get; set; }
        public bool IsReached { get; set; }
        public DateTime? ReachedAt { get; set; }
    }

    /// <summary>
    /// Summary of a goal transaction (contribution/withdrawal)
    /// </summary>
    public class GoalTransactionSummary
    {
        public Guid TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty; // "Contribution" or "Withdrawal"
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }

    /// <summary>
    /// Paginated list of goals
    /// </summary>
    public class GoalListResponse
    {
        public List<GoalSummaryResponse> Goals { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        
        // Summary Stats
        public decimal TotalSaved { get; set; }
        public decimal TotalTarget { get; set; }
        public int ActiveGoalsCount { get; set; }
        public int CompletedGoalsCount { get; set; }
    }

    /// <summary>
    /// Platform-wide goal statistics (Admin)
    /// </summary>
    public class GoalStatsResponse
    {
        public int TotalGoals { get; set; }
        public int ActiveGoals { get; set; }
        public int CompletedGoals { get; set; }
        public int PausedGoals { get; set; }
        public int CancelledGoals { get; set; }
        
        // By Type
        public int EmergencyGoals { get; set; }
        public int InvestmentGoals { get; set; }
        public int CustomGoals { get; set; }
        
        // Financial Data
        public decimal TotalAmountSaved { get; set; }
        public decimal TotalTargetAmount { get; set; }
        public decimal AverageGoalSize { get; set; }
        public decimal AverageCompletionRate { get; set; }
        
        // Performance
        public int GoalsCompletedThisMonth { get; set; }
        public decimal TotalContributionsThisMonth { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }
}
