using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
    {
        public void Configure(EntityTypeBuilder<Portfolio> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.UserId);
            builder.HasIndex(p => p.ConsultantId); // Per query consulente ? portfolios clienti

            builder.Property(p => p.TotalInvested).HasPrecision(18, 2);
            builder.Property(p => p.CurrentValue).HasPrecision(18, 2);

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Consultant)
                .WithMany()
                .HasForeignKey(p => p.ConsultantId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
