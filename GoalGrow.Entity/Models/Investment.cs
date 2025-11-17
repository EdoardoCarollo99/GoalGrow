using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta un investimento effettuato da un utente su un prodotto finanziario
    /// </summary>
    public class Investment
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid PortfolioId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        public Guid? ConsultantId { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvestmentNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvestmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal Quantity { get; set; } = 0m; // Quantità acquistata (azioni, quote, ecc.)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; } = 0m; // Prezzo di acquisto unitario

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = 0m; // Importo totale investito

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fee { get; set; } // Commissioni

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ConsultantFee { get; set; } // Commissione del consulente

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "EUR";

        [Required]
        public InvestmentStatus Status { get; set; } = InvestmentStatus.Active;

        public DateTime? SoldDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SellPrice { get; set; } // Prezzo di vendita unitario

        [Column(TypeName = "decimal(18,2)")]
        public decimal? SellAmount { get; set; } // Importo totale dalla vendita

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        [ForeignKey(nameof(PortfolioId))]
        public virtual Portfolio Portfolio { get; set; } = default!;

        [ForeignKey(nameof(ProductId))]
        public virtual InvestmentProduct Product { get; set; } = default!;

        [ForeignKey(nameof(ConsultantId))]
        public virtual ConsultantUser? Consultant { get; set; }

        public Investment(Guid userId, Guid portfolioId, Guid productId, string investmentNumber,
            decimal quantity, decimal purchasePrice, decimal totalAmount)
        {
            UserId = userId;
            PortfolioId = portfolioId;
            ProductId = productId;
            InvestmentNumber = investmentNumber;
            Quantity = quantity;
            PurchasePrice = purchasePrice;
            TotalAmount = totalAmount;
            InvestmentDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }

        // Proprietà calcolate
        [NotMapped]
        public decimal CurrentValue { get; set; } // Deve essere calcolato in base al prezzo corrente del prodotto

        [NotMapped]
        public decimal ProfitLoss => (SellAmount ?? CurrentValue) - TotalAmount;

        [NotMapped]
        public decimal ProfitLossPercentage => TotalAmount > 0 ? (ProfitLoss / TotalAmount) * 100 : 0;
    }
}
