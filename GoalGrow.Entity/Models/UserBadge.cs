using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta un badge guadagnato da un utente
    /// </summary>
    public class UserBadge
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid BadgeId { get; set; }

        [Required]
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

        public int ProgressPercentage { get; set; } = 100; // Per badge progressivi

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(BadgeId))]
        public virtual Badge Badge { get; set; } = default!;

        public UserBadge(Guid userId, Guid badgeId)
        {
            UserId = userId;
            BadgeId = badgeId;
            EarnedAt = DateTime.UtcNow;
        }
    }
}
