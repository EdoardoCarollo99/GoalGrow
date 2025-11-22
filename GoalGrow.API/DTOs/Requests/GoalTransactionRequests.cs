using System.ComponentModel.DataAnnotations;

namespace GoalGrow.API.DTOs.Requests
{
    /// <summary>
    /// Request for contributing funds to a goal
    /// </summary>
    public class GoalContributionRequest
    {
        /// <summary>
        /// Amount to contribute (in EUR)
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between €0.01 and €100,000")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Optional note for this contribution
        /// </summary>
        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// Request for withdrawing funds from a goal
    /// </summary>
    public class GoalWithdrawalRequest
    {
        /// <summary>
        /// Amount to withdraw (in EUR)
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 100000, ErrorMessage = "Amount must be between €0.01 and €100,000")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Reason for withdrawal (required for tracking)
        /// </summary>
        [Required(ErrorMessage = "Withdrawal reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;
    }
}
