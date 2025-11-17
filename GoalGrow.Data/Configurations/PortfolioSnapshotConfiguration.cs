using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class PortfolioSnapshotConfiguration : IEntityTypeConfiguration<PortfolioSnapshot>
    {
        public void Configure(EntityTypeBuilder<PortfolioSnapshot> builder)
        {
            builder.HasKey(ps => ps.Id);

            builder.HasIndex(ps => new { ps.PortfolioId, ps.SnapshotDate });

            builder.Property(ps => ps.TotalValue).HasPrecision(18, 2);
            builder.Property(ps => ps.TotalInvested).HasPrecision(18, 2);
            builder.Property(ps => ps.TotalReturn).HasPrecision(18, 2);
            builder.Property(ps => ps.ReturnPercentage).HasPrecision(18, 4);
            builder.Property(ps => ps.DividendsReceived).HasPrecision(18, 2);

            // Relationships
            builder.HasOne(ps => ps.Portfolio)
                .WithMany()
                .HasForeignKey(ps => ps.PortfolioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ps => ps.User)
                .WithMany()
                .HasForeignKey(ps => ps.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
