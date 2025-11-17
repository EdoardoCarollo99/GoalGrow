using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    public class Budget
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public BudgetPeriod Period { get; set; } = BudgetPeriod.Monthly;

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? EndDate { get; set; }

        [Required]
        public BudgetStatus Status { get; set; } = BudgetStatus.Active;

        public bool SendAlerts { get; set; } = true;

        [Range(0, 100)]
        public int AlertThreshold { get; set; } = 80; // Alert when 80% spent

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public Budget(string name, string category, decimal amount, BudgetPeriod period, 
            DateTime startDate, Guid userId)
        {
            Name = name;
            Category = category;
            Amount = amount;
            Period = period;
            StartDate = startDate;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public decimal RemainingAmount => Amount - SpentAmount;

        [NotMapped]
        public decimal PercentageUsed => Amount > 0 ? (SpentAmount / Amount) * 100 : 0;

        [NotMapped]
        public bool IsOverBudget => SpentAmount > Amount;

        [NotMapped]
        public bool ShouldAlert => SendAlerts && PercentageUsed >= AlertThreshold;
    }
}
