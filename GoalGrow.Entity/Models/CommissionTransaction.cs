using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta una commissione pagata a un consulente
    /// </summary>
    public class CommissionTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(50)]
        public string CommissionNumber { get; set; } = string.Empty;

        [Required]
        public Guid ConsultantId { get; set; }

        [Required]
        public Guid InvestorUserId { get; set; }

        public Guid? InvestmentId { get; set; } // Investimento che ha generato la commissione

        [Required]
        public CommissionType Type { get; set; } = CommissionType.Investment;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseAmount { get; set; } = 0m; // Importo su cui è calcolata la commissione

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal CommissionRate { get; set; } = 0m; // Percentuale di commissione

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public DateTime? PaymentDate { get; set; }

        [Required]
        public CommissionStatus Status { get; set; } = CommissionStatus.Pending;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(ConsultantId))]
        public virtual ConsultantUser Consultant { get; set; } = default!;

        [ForeignKey(nameof(InvestorUserId))]
        public virtual InversotorUser InvestorUser { get; set; } = default!;

        [ForeignKey(nameof(InvestmentId))]
        public virtual Investment? Investment { get; set; }

        public CommissionTransaction(string commissionNumber, Guid consultantId, Guid investorUserId,
            CommissionType type, decimal baseAmount, decimal commissionRate)
        {
            CommissionNumber = commissionNumber;
            ConsultantId = consultantId;
            InvestorUserId = investorUserId;
            Type = type;
            BaseAmount = baseAmount;
            CommissionRate = commissionRate;
            Amount = baseAmount * (commissionRate / 100);
            TransactionDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
