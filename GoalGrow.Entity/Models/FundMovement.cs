using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta il movimento di fondi tra il conto privato dell'utente e il conto aziendale
    /// </summary>
    public class FundMovement
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(50)]
        public string MovementNumber { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid UserAccountId { get; set; } // Conto privato dell'utente

        [Required]
        public Guid CompanyAccountId { get; set; } // Conto aziendale

        [Required]
        public FundMovementType Type { get; set; } = FundMovementType.Deposit;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [Required]
        public FundMovementStatus Status { get; set; } = FundMovementStatus.Pending;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        [MaxLength(200)]
        public string RejectionReason { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ExternalReference { get; set; } = string.Empty; // Riferimento bonifico bancario

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(UserAccountId))]
        public virtual Account UserAccount { get; set; } = default!;

        [ForeignKey(nameof(CompanyAccountId))]
        public virtual CompanyAccount CompanyAccount { get; set; } = default!;

        public FundMovement(string movementNumber, Guid userId, Guid userAccountId, 
            Guid companyAccountId, FundMovementType type, decimal amount)
        {
            MovementNumber = movementNumber;
            UserId = userId;
            UserAccountId = userAccountId;
            CompanyAccountId = companyAccountId;
            Type = type;
            Amount = amount;
            RequestDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
