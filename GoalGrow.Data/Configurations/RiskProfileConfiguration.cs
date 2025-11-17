using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class RiskProfileConfiguration : IEntityTypeConfiguration<RiskProfile>
    {
        public void Configure(EntityTypeBuilder<RiskProfile> builder)
        {
            builder.HasKey(rp => rp.Id);

            builder.HasIndex(rp => rp.UserId).IsUnique();

            builder.Property(rp => rp.AnnualIncome).HasPrecision(18, 2);
            builder.Property(rp => rp.NetWorth).HasPrecision(18, 2);
            builder.Property(rp => rp.LiquidAssets).HasPrecision(18, 2);

            // Relationship
            builder.HasOne(rp => rp.User)
                .WithOne(iu => iu.RiskProfile)
                .HasForeignKey<RiskProfile>(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
