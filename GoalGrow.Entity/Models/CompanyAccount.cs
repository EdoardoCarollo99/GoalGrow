using GoalGrow.Entity.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta il conto centralizzato dell'azienda dove confluiscono i depositi degli utenti
    /// </summary>
    public class CompanyAccount
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string AccountName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string AccountNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string IBAN { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AvailableBalance { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDeposits { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalWithdrawals { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvested { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Active;

        [MaxLength(100)]
        public string BankName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string BankCode { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<FundMovement> FundMovements { get; set; } = new List<FundMovement>();

        public CompanyAccount(string accountName, string accountNumber)
        {
            AccountName = accountName;
            AccountNumber = accountNumber;
            CreatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public decimal NetBalance => TotalDeposits - TotalWithdrawals;

        [NotMapped]
        public decimal UnallocatedFunds => Balance - TotalInvested;
    }
}
