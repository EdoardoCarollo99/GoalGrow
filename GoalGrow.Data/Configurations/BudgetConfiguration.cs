using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
    {
        public void Configure(EntityTypeBuilder<Budget> builder)
        {
            builder.HasKey(b => b.Id);

            builder.HasIndex(b => new { b.UserId, b.Status });

            builder.Property(b => b.Amount).HasPrecision(18, 2);
            builder.Property(b => b.SpentAmount).HasPrecision(18, 2);

            // Relationship
            builder.HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
