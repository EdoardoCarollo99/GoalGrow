using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(100)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string AccountName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public AccountType Type { get; set; } = AccountType.Checking;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AvailableBalance { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string BankCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IBAN { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Account(string accountNumber, string accountName, AccountType type, Guid userId)
        {
            AccountNumber = accountNumber;
            AccountName = accountName;
            Type = type;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            AvailableBalance = 0m;
        }
    }
}
