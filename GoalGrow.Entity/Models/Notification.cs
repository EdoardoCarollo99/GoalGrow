using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta una notifica inviata a un utente
    /// </summary>
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; } = NotificationType.Info;

        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }

        [MaxLength(500)]
        public string ActionUrl { get; set; } = string.Empty; // URL per azione (es: "/investments/123")

        [MaxLength(100)]
        public string ActionLabel { get; set; } = string.Empty; // Etichetta del bottone (es: "Visualizza")

        // Riferimenti opzionali a entità correlate
        public Guid? RelatedEntityId { get; set; }

        [MaxLength(100)]
        public string RelatedEntityType { get; set; } = string.Empty; // "Goal", "Investment", "Budget", etc.

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiresAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public Notification(Guid userId, string title, string message, NotificationType type)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        [NotMapped]
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    }
}
