using System.ComponentModel.DataAnnotations;

namespace GoalGrow.API.DTOs.Requests
{
    /// <summary>
    /// Request for creating a new savings goal
    /// </summary>
    public class CreateGoalRequest
    {
        /// <summary>
        /// Goal name (e.g., "New Car", "House Down Payment")
        /// </summary>
        [Required(ErrorMessage = "Goal name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Detailed description of the goal
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Type of goal (Emergency, Investment, Custom)
        /// </summary>
        [Required(ErrorMessage = "Goal type is required")]
        public string Type { get; set; } = "Custom";

        /// <summary>
        /// Target amount to save (in EUR)
        /// </summary>
        [Required(ErrorMessage = "Target amount is required")]
        [Range(1, 1000000, ErrorMessage = "Target amount must be between €1 and €1,000,000")]
        public decimal TargetAmount { get; set; }

        /// <summary>
        /// Target date to achieve the goal
        /// </summary>
        [Required(ErrorMessage = "Target date is required")]
        public DateTime TargetDate { get; set; }

        /// <summary>
        /// Initial contribution amount (optional)
        /// </summary>
        [Range(0, 1000000, ErrorMessage = "Initial amount cannot exceed €1,000,000")]
        public decimal? InitialAmount { get; set; }

        /// <summary>
        /// Priority level (Low, Medium, High, Critical)
        /// </summary>
        public string? Priority { get; set; } = "Medium";

        /// <summary>
        /// Category for custom goals (e.g., "Travel", "Education", "Wedding")
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Icon URL for visual representation
        /// </summary>
        [StringLength(500)]
        [Url(ErrorMessage = "Icon URL must be a valid URL")]
        public string? IconUrl { get; set; }

        /// <summary>
        /// Enable automatic savings
        /// </summary>
        public bool EnableAutoSave { get; set; } = false;

        /// <summary>
        /// Auto-save amount (if auto-save enabled)
        /// </summary>
        [Range(0, 10000, ErrorMessage = "Auto-save amount cannot exceed €10,000")]
        public decimal? AutoSaveAmount { get; set; }

        /// <summary>
        /// Auto-save frequency (Daily, Weekly, Monthly)
        /// </summary>
        public string? AutoSaveFrequency { get; set; }

        /// <summary>
        /// Lock withdrawals until goal is reached
        /// </summary>
        public bool LockWithdrawals { get; set; } = false;
    }
}
