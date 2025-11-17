using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta uno snapshot periodico della performance di un portafoglio
    /// </summary>
    public class PortfolioSnapshot
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid PortfolioId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;

        [Required]
        public SnapshotPeriod Period { get; set; } = SnapshotPeriod.Daily;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalValue { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvested { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalReturn { get; set; } = 0m;

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ReturnPercentage { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DividendsReceived { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        public int ActiveInvestmentsCount { get; set; } = 0;

        [MaxLength(2000)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(PortfolioId))]
        public virtual Portfolio Portfolio { get; set; } = default!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public PortfolioSnapshot(Guid portfolioId, Guid userId, decimal totalValue, 
            decimal totalInvested, SnapshotPeriod period)
        {
            PortfolioId = portfolioId;
            UserId = userId;
            TotalValue = totalValue;
            TotalInvested = totalInvested;
            TotalReturn = totalValue - totalInvested;
            ReturnPercentage = totalInvested > 0 ? ((totalValue - totalInvested) / totalInvested) * 100 : 0;
            Period = period;
            SnapshotDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
