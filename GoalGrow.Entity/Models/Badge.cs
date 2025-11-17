using GoalGrow.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta un badge/distintivo che può essere guadagnato dagli utenti
    /// </summary>
    public class Badge
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // Es: "FIRST_GOAL", "INVESTOR_PRO"

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string IconUrl { get; set; } = string.Empty;

        [Required]
        public BadgeCategory Category { get; set; } = BadgeCategory.Achievement;

        [Required]
        public BadgeRarity Rarity { get; set; } = BadgeRarity.Common;

        public int PointsReward { get; set; } = 0; // Punti XP assegnati quando si ottiene il badge

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string Requirements { get; set; } = string.Empty; // Descrizione requisiti

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();

        public Badge(string code, string name, string description, BadgeRarity rarity)
        {
            Code = code;
            Name = name;
            Description = description;
            Rarity = rarity;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
