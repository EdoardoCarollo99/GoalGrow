using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(50)]
        public string TransactionNumber { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; } = default!;

        // Dati temporali
        [Required]
        public DateTime TransactionDate { get; set; } = default!;

        public DateTime? ValueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0m;
        
        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; } = 0m;
        
        [Required]
        public Guid AccountId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        public Guid? PayeeId { get; set; }
        
        public Guid? GoalId { get; set; }

        public Guid? RecurringTransactionId { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Notes { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Tags { get; set; } = string.Empty;
        
        [Required]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        
        [MaxLength(100)]
        public string Reference { get; set; } = string.Empty;

        public bool IsReconciled { get; set; } = false;

        public DateTime? ReconciledAt { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; } = default!;

        [ForeignKey(nameof(PayeeId))]
        public virtual Payee? Payee { get; set; }

        [ForeignKey(nameof(GoalId))]
        public virtual Goal? Goal { get; set; }

        [ForeignKey(nameof(RecurringTransactionId))]
        public virtual RecurringTransaction? RecurringTransaction { get; set; }

        public Transaction(string transactionNumber, TransactionType type, DateTime transactionDate, 
            decimal amount, Guid accountId, Guid userId, decimal balanceAfter)
        {
            TransactionNumber = transactionNumber;
            Type = type;
            TransactionDate = transactionDate;
            Amount = amount;
            AccountId = accountId;
            UserId = userId;
            BalanceAfter = balanceAfter;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
