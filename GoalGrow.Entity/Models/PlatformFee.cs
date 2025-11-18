using GoalGrow.Entity.Common;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta le fee applicate da GoalGrow su transazioni/investimenti
    /// (diverse dalle commissioni dei consulenti)
    /// </summary>
    public class PlatformFee : AuditableEntity
    {
        [Required]
        [MaxLength(50)]
        public string FeeNumber { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public PlatformFeeType Type { get; set; } = PlatformFeeType.Deposit;

        // Riferimento alla transazione/investimento che ha generato la fee
        public Guid? RelatedTransactionId { get; set; }
        public Guid? RelatedInvestmentId { get; set; }
        public Guid? RelatedFundMovementId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseAmount { get; set; } = 0m; // Importo su cui calcolare la fee

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal FeePercentage { get; set; } = 0m; // Percentuale fee (es. 1.00 = 1%)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumFee { get; set; } = 0m; // Fee minima (es. 1€)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CalculatedFee { get; set; } = 0m; // Fee effettiva applicata

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public DateTime? CollectedAt { get; set; }

        [Required]
        public FeeStatus Status { get; set; } = FeeStatus.Pending;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(RelatedTransactionId))]
        public virtual Transaction? RelatedTransaction { get; set; }

        [ForeignKey(nameof(RelatedInvestmentId))]
        public virtual Investment? RelatedInvestment { get; set; }

        [ForeignKey(nameof(RelatedFundMovementId))]
        public virtual FundMovement? RelatedFundMovement { get; set; }

        public PlatformFee(string feeNumber, Guid userId, PlatformFeeType type, decimal baseAmount, decimal feePercentage, decimal minimumFee)
        {
            FeeNumber = feeNumber;
            UserId = userId;
            Type = type;
            BaseAmount = baseAmount;
            FeePercentage = feePercentage;
            MinimumFee = minimumFee;
            CalculatedFee = CalculateFee(baseAmount, feePercentage, minimumFee);
            TransactionDate = DateTime.UtcNow;
        }

        // Helper method per calcolare fee
        private static decimal CalculateFee(decimal baseAmount, decimal feePercentage, decimal minimumFee)
        {
            var calculatedFee = baseAmount * (feePercentage / 100);
            return Math.Max(calculatedFee, minimumFee);
        }

        // Computed
        [NotMapped]
        public bool IsCollected => Status == FeeStatus.Collected;
    }
}
