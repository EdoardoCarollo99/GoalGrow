using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta i punti esperienza (XP) e il livello di un utente
    /// </summary>
    public class UserLevel
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public int CurrentLevel { get; set; } = 1;

        [Required]
        public int TotalPoints { get; set; } = 0;

        [Required]
        public int CurrentLevelPoints { get; set; } = 0; // Punti nel livello corrente

        [Required]
        public int PointsToNextLevel { get; set; } = 100; // Punti necessari per il prossimo livello

        public DateTime? LastLevelUpAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        private UserLevel()
        {
            CurrentLevel = 1;
            TotalPoints = 0;
            CurrentLevelPoints = 0;
            PointsToNextLevel = 100;
            CreatedAt = DateTime.UtcNow;
        }

        public UserLevel(Guid userId) : this()
        {
            UserId = userId;
        }

        [NotMapped]
        public int ProgressPercentage => PointsToNextLevel > 0 
            ? (int)((decimal)CurrentLevelPoints / PointsToNextLevel * 100) 
            : 0;
    }
}
