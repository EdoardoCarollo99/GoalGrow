using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta il portafoglio investimenti di un utente
    /// </summary>
    public class Portfolio
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public Guid UserId { get; set; }

        public Guid? ConsultantId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvested { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentValue { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(ConsultantId))]
        public virtual ConsultantUser? Consultant { get; set; }

        public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();

        public Portfolio(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }

        // Proprietà calcolate
        [NotMapped]
        public decimal TotalReturn => CurrentValue - TotalInvested;

        [NotMapped]
        public decimal ReturnPercentage => TotalInvested > 0 ? ((CurrentValue - TotalInvested) / TotalInvested) * 100 : 0;
    }
}
