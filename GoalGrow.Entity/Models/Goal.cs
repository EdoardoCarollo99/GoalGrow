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

        // NEW: Tipo di obiettivo
        [Required]
        public GoalType Type { get; set; } = GoalType.Custom;

        // NEW: Flag per obiettivi di sistema (Emergenze, Investimenti)
        public bool IsSystemGoal { get; set; } = false;

        // NEW: Soglia minima per sbloccare funzionalità (es. 5000€ per investimenti)
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnlockThreshold { get; set; }

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

        [MaxLength(500)]
        public string IconUrl { get; set; } = string.Empty;

        // Auto-save configuration
        public bool IsAutoSave { get; set; } = false;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AutoSaveAmount { get; set; } = 0m;

        public RecurrenceFrequency? AutoSaveFrequency { get; set; }

        // NEW: Possibilità di bloccare/sbloccare prelievi
        public bool IsWithdrawalLocked { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public DateTime TargetDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        [Required]
        public GoalStatus Status { get; set; } = GoalStatus.Active;

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Goal(string name, decimal targetAmount, DateTime targetDate, Guid userId, GoalType type = GoalType.Custom)
        {
            Name = name;
            TargetAmount = targetAmount;
            TargetDate = targetDate;
            UserId = userId;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

        [NotMapped]
        public decimal RemainingAmount => TargetAmount - CurrentAmount;

        [NotMapped]
        public bool IsCompleted => CurrentAmount >= TargetAmount;

        [NotMapped]
        public int DaysRemaining => (TargetDate - DateTime.UtcNow).Days;

        [NotMapped]
        public bool HasReachedUnlockThreshold => UnlockThreshold.HasValue && CurrentAmount >= UnlockThreshold.Value;

        // NEW: Helper method per obiettivi di sistema
        public static Goal CreateEmergencyGoal(Guid userId, decimal targetAmount)
        {
            return new Goal("Fondo di Emergenza", targetAmount, DateTime.UtcNow.AddYears(1), userId, GoalType.Emergency)
            {
                IsSystemGoal = true,
                Description = "Fondo di sicurezza per emergenze impreviste. Consigliato: 3-6 mesi di spese.",
                Priority = GoalPriority.High,
                IsWithdrawalLocked = false // Può essere svincolato in emergenza
            };
        }

        public static Goal CreateInvestmentGoal(Guid userId, decimal unlockThreshold = 5000m)
        {
            return new Goal("Fondo Investimenti", unlockThreshold * 2, DateTime.UtcNow.AddYears(2), userId, GoalType.Investment)
            {
                IsSystemGoal = true,
                UnlockThreshold = unlockThreshold,
                Description = $"Raggiungi {unlockThreshold:C} per sbloccare il marketplace dei consulenti finanziari.",
                Priority = GoalPriority.High,
                IsWithdrawalLocked = true // Bloccato fino al raggiungimento soglia
            };
        }
    }
}
