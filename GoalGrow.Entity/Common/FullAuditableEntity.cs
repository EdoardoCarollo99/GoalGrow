using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.Common
{
    /// <summary>
    /// Classe base per entità di audit completo (creazione, modifica, eliminazione soft)
    /// </summary>
    public abstract class FullAuditableEntity : AuditableEntity
    {
        public DateTime? DeletedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public void SoftDelete()
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
