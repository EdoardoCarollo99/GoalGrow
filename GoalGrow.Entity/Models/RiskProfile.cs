using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta il profilo di rischio di un utente investitore (MIFID II compliance)
    /// </summary>
    public class RiskProfile
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public RiskTolerance RiskTolerance { get; set; } = RiskTolerance.Moderate;

        [Required]
        public InvestmentExperience InvestmentExperience { get; set; } = InvestmentExperience.Beginner;

        [Required]
        public InvestmentObjective InvestmentObjective { get; set; } = InvestmentObjective.Growth;

        [Required]
        public InvestmentHorizon InvestmentHorizon { get; set; } = InvestmentHorizon.Medium;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal AnnualIncome { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetWorth { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal LiquidAssets { get; set; } = 0m;

        [Range(0, 100)]
        public int RiskScore { get; set; } = 50; // Punteggio di rischio da 0 (conservativo) a 100 (aggressivo)

        public DateTime ProfileDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastReviewDate { get; set; }

        public DateTime? NextReviewDate { get; set; } // Revisione annuale obbligatoria

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;

        public bool RequiresMaintenance { get; set; } = false; // Se richiede aggiornamento

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual InversotorUser User { get; set; } = default!;

        // Costruttore parameterless per EF Core
        private RiskProfile()
        {
            ProfileDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            NextReviewDate = DateTime.UtcNow.AddYears(1);
        }

        public RiskProfile(Guid userId, RiskTolerance riskTolerance, InvestmentExperience experience)
        {
            UserId = userId;
            RiskTolerance = riskTolerance;
            InvestmentExperience = experience;
            ProfileDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            // Revisione annuale
            NextReviewDate = DateTime.UtcNow.AddYears(1);
        }

        [NotMapped]
        public bool IsExpired => NextReviewDate.HasValue && NextReviewDate.Value < DateTime.UtcNow;

        [NotMapped]
        public string RiskCategory => RiskScore switch
        {
            <= 20 => "Molto Conservativo",
            <= 40 => "Conservativo",
            <= 60 => "Moderato",
            <= 80 => "Aggressivo",
            _ => "Molto Aggressivo"
        };
    }
}
