using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class FundMovementConfiguration : IEntityTypeConfiguration<FundMovement>
    {
        public void Configure(EntityTypeBuilder<FundMovement> builder)
        {
            builder.HasKey(fm => fm.Id);

            builder.HasIndex(fm => fm.MovementNumber).IsUnique();
            builder.HasIndex(fm => new { fm.UserId, fm.Status });

            builder.Property(fm => fm.Amount).HasPrecision(18, 2);
            builder.Property(fm => fm.Fee).HasPrecision(18, 2);

            // Relationships
            builder.HasOne(fm => fm.User)
                .WithMany()
                .HasForeignKey(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fm => fm.UserAccount)
                .WithMany()
                .HasForeignKey(fm => fm.UserAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fm => fm.CompanyAccount)
                .WithMany(ca => ca.FundMovements)
                .HasForeignKey(fm => fm.CompanyAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
