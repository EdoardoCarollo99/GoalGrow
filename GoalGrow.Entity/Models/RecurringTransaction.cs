using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    public class RecurringTransaction
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public TransactionType Type { get; set; } = default!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; }

        [Required]
        public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Monthly;

        [Required]
        public int Interval { get; set; } = 1; // Every X periods (e.g., every 2 weeks)

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        public DateTime? NextExecutionDate { get; set; }

        public DateTime? LastExecutionDate { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Guid? PayeeId { get; set; }

        public Guid? GoalId { get; set; }

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Tags { get; set; } = string.Empty;

        [Required]
        public RecurringTransactionStatus Status { get; set; } = RecurringTransactionStatus.Active;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(AccountId))]
        public virtual Account Account { get; set; } = default!;

        [ForeignKey(nameof(PayeeId))]
        public virtual Payee? Payee { get; set; }

        [ForeignKey(nameof(GoalId))]
        public virtual Goal? Goal { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public RecurringTransaction(string name, TransactionType type, decimal amount, 
            RecurrenceFrequency frequency, DateTime startDate, Guid accountId, Guid userId)
        {
            Name = name;
            Type = type;
            Amount = amount;
            Frequency = frequency;
            StartDate = startDate;
            AccountId = accountId;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            NextExecutionDate = startDate;
        }
    }
}
