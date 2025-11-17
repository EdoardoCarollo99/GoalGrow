using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class RecurringTransactionConfiguration : IEntityTypeConfiguration<RecurringTransaction>
    {
        public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.HasIndex(rt => new { rt.UserId, rt.Status });
            builder.HasIndex(rt => rt.NextExecutionDate);

            builder.Property(rt => rt.Amount).HasPrecision(18, 2);
            builder.Property(rt => rt.Fee).HasPrecision(18, 2);

            // Relationships
            builder.HasOne(rt => rt.Account)
                .WithMany()
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(rt => rt.Payee)
                .WithMany()
                .HasForeignKey(rt => rt.PayeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(rt => rt.Goal)
                .WithMany()
                .HasForeignKey(rt => rt.GoalId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
