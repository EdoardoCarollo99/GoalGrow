using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class UserLevelConfiguration : IEntityTypeConfiguration<UserLevel>
    {
        public void Configure(EntityTypeBuilder<UserLevel> builder)
        {
            builder.HasKey(ul => ul.Id);

            builder.HasIndex(ul => ul.UserId).IsUnique();

            // Relationship
            builder.HasOne(ul => ul.User)
                .WithMany()
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
