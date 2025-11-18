using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.Common
{
    /// <summary>
    /// Classe base per entità con campi di audit
    /// </summary>
    public abstract class AuditableEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
