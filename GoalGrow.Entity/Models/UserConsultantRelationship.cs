using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta la relazione tra un utente investitore e il suo consulente finanziario
    /// </summary>
    public class UserConsultantRelationship
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid InvestorUserId { get; set; }

        [Required]
        public Guid ConsultantUserId { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public DateTime? TerminatedDate { get; set; }

        [Required]
        public RelationshipStatus Status { get; set; } = RelationshipStatus.Active;

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        // Rating del consulente da parte dell'utente
        [Range(1, 5)]
        public int? Rating { get; set; }

        [MaxLength(1000)]
        public string Review { get; set; } = string.Empty;

        public DateTime? ReviewDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(InvestorUserId))]
        public virtual InversotorUser InvestorUser { get; set; } = default!;

        [ForeignKey(nameof(ConsultantUserId))]
        public virtual ConsultantUser ConsultantUser { get; set; } = default!;

        public UserConsultantRelationship(Guid investorUserId, Guid consultantUserId)
        {
            InvestorUserId = investorUserId;
            ConsultantUserId = consultantUserId;
            AssignedDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
