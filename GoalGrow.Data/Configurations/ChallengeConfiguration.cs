using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class ChallengeConfiguration : IEntityTypeConfiguration<Challenge>
    {
        public void Configure(EntityTypeBuilder<Challenge> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.MoneyReward).HasPrecision(18, 2);

            // Relationship
            builder.HasOne(c => c.BadgeReward)
                .WithMany()
                .HasForeignKey(c => c.BadgeRewardId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
