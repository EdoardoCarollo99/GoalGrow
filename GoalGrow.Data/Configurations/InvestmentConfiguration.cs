using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class InvestmentConfiguration : IEntityTypeConfiguration<Investment>
    {
        public void Configure(EntityTypeBuilder<Investment> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasIndex(i => i.InvestmentNumber)
                .IsUnique();

            builder.HasIndex(i => new { i.UserId, i.Status });
            builder.HasIndex(i => new { i.PortfolioId, i.Status });

            builder.Property(i => i.Quantity)
                .HasPrecision(18, 6);

            builder.Property(i => i.PurchasePrice)
                .HasPrecision(18, 2);

            builder.Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            builder.Property(i => i.SellPrice)
                .HasPrecision(18, 2);

            builder.Property(i => i.SellAmount)
                .HasPrecision(18, 2);

            // Relationships
            builder.HasOne(i => i.Portfolio)
                .WithMany(p => p.Investments)
                .HasForeignKey(i => i.PortfolioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Product)
                .WithMany(ip => ip.Investments)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
