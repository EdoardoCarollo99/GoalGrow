using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class UserChallengeConfiguration : IEntityTypeConfiguration<UserChallenge>
    {
        public void Configure(EntityTypeBuilder<UserChallenge> builder)
        {
            builder.HasKey(uc => uc.Id);

            builder.HasIndex(uc => new { uc.UserId, uc.Status });

            // Relationships
            builder.HasOne(uc => uc.User)
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uc => uc.Challenge)
                .WithMany(c => c.UserChallenges)
                .HasForeignKey(uc => uc.ChallengeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
