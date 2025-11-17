using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class GoalConfiguration : IEntityTypeConfiguration<Goal>
    {
        public void Configure(EntityTypeBuilder<Goal> builder)
        {
            builder.HasKey(g => g.Id);

            builder.HasIndex(g => new { g.UserId, g.Status });

            builder.Property(g => g.TargetAmount).HasPrecision(18, 2);
            builder.Property(g => g.CurrentAmount).HasPrecision(18, 2);

            // Relationship
            builder.HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
