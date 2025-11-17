using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    public class Goal
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TargetAmount { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentAmount { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public GoalPriority Priority { get; set; } = GoalPriority.Medium;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? TargetDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        public GoalStatus Status { get; set; } = GoalStatus.Active;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Goal(string name, decimal targetAmount, Guid userId)
        {
            Name = name;
            TargetAmount = targetAmount;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

        [NotMapped]
        public decimal RemainingAmount => TargetAmount - CurrentAmount;

        [NotMapped]
        public bool IsCompleted => CurrentAmount >= TargetAmount;

        [NotMapped]
        public int? DaysRemaining => TargetDate.HasValue ? (TargetDate.Value - DateTime.UtcNow).Days : null;
    }
}
