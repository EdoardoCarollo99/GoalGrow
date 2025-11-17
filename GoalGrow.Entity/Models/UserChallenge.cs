using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta una sfida accettata da un utente
    /// </summary>
    public class UserChallenge
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ChallengeId { get; set; }

        [Required]
        public DateTime AcceptedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }

        [Required]
        public ChallengeStatus Status { get; set; } = ChallengeStatus.InProgress;

        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(ChallengeId))]
        public virtual Challenge Challenge { get; set; } = default!;

        public UserChallenge(Guid userId, Guid challengeId)
        {
            UserId = userId;
            ChallengeId = challengeId;
            AcceptedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public bool IsCompleted => Status == ChallengeStatus.Completed;
    }
}
