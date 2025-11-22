using System.ComponentModel.DataAnnotations;

namespace GoalGrow.API.DTOs.Requests
{
    /// <summary>
    /// Request for updating an existing goal
    /// </summary>
    public class UpdateGoalRequest
    {
        /// <summary>
        /// Updated goal name
        /// </summary>
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 200 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// Updated description
        /// </summary>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        /// <summary>
        /// Updated target amount
        /// </summary>
        [Range(1, 1000000, ErrorMessage = "Target amount must be between €1 and €1,000,000")]
        public decimal? TargetAmount { get; set; }

        /// <summary>
        /// Updated target date
        /// </summary>
        public DateTime? TargetDate { get; set; }

        /// <summary>
        /// Updated priority
        /// </summary>
        public string? Priority { get; set; }

        /// <summary>
        /// Updated category
        /// </summary>
        [StringLength(100)]
        public string? Category { get; set; }

        /// <summary>
        /// Updated icon URL
        /// </summary>
        [StringLength(500)]
        [Url(ErrorMessage = "Icon URL must be a valid URL")]
        public string? IconUrl { get; set; }

        /// <summary>
        /// Update auto-save settings
        /// </summary>
        public bool? EnableAutoSave { get; set; }

        /// <summary>
        /// Updated auto-save amount
        /// </summary>
        [Range(0, 10000, ErrorMessage = "Auto-save amount cannot exceed €10,000")]
        public decimal? AutoSaveAmount { get; set; }

        /// <summary>
        /// Updated auto-save frequency
        /// </summary>
        public string? AutoSaveFrequency { get; set; }

        /// <summary>
        /// Update withdrawal lock status
        /// </summary>
        public bool? LockWithdrawals { get; set; }
    }
}
