using GoalGrow.Entity.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta un prodotto finanziario disponibile per l'investimento
    /// </summary>
    public class InvestmentProduct
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(100)]
        public string Code { get; set; } = string.Empty; // Es: "AAPL", "BTC-EUR", "FUND001"

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public InvestmentProductType Type { get; set; } = InvestmentProductType.Stock;

        [Required]
        public RiskLevel RiskLevel { get; set; } = RiskLevel.Medium;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentPrice { get; set; } = 0m;

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinimumInvestment { get; set; } = 0m;

        public bool IsActive { get; set; } = true;

        [MaxLength(200)]
        public string Issuer { get; set; } = string.Empty; // Emittente

        [MaxLength(100)]
        public string ISIN { get; set; } = string.Empty; // Codice ISIN internazionale

        // Rendimenti storici
        [Column(TypeName = "decimal(18,4)")]
        public decimal? YearlyReturn { get; set; } // Rendimento annuale medio

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ExpectedReturn { get; set; } // Rendimento atteso

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? LastPriceUpdate { get; set; }

        public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();

        public InvestmentProduct(string code, string name, InvestmentProductType type, decimal currentPrice)
        {
            Code = code;
            Name = name;
            Type = type;
            CurrentPrice = currentPrice;
            CreatedAt = DateTime.UtcNow;
            LastPriceUpdate = DateTime.UtcNow;
        }
    }
}
