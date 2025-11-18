using GoalGrow.Entity.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta una sfida finanziaria che gli utenti possono completare
    /// </summary>
    public class Challenge
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ChallengeType Type { get; set; } = ChallengeType.Saving;

        [Required]
        public ChallengeDifficulty Difficulty { get; set; } = ChallengeDifficulty.Easy;

        public int PointsReward { get; set; } = 0;

        public Guid? BadgeRewardId { get; set; } // Badge assegnato al completamento

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MoneyReward { get; set; } // Ricompensa monetaria (es: cashback)

        [Required]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsRecurring { get; set; } = false; // Se ricorre ogni mese/settimana

        [MaxLength(500)]
        public string Requirements { get; set; } = string.Empty; // Descrizione requisiti

        [MaxLength(500)]
        public string IconUrl { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Target properties for challenge completion
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TargetAmount { get; set; } // Target amount for money-based challenges

        public int? TargetCount { get; set; } // Target count for count-based challenges

        // Navigation properties
        [ForeignKey(nameof(BadgeRewardId))]
        public virtual Badge? BadgeReward { get; set; }

        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();

        // Alias for backward compatibility
        [NotMapped]
        public int RewardXP => PointsReward;

        public Challenge(string title, string description, ChallengeType type, DateTime endDate)
        {
            Title = title;
            Description = description;
            Type = type;
            EndDate = endDate;
            StartDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
