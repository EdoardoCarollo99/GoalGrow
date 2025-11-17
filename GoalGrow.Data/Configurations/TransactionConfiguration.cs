using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.HasIndex(t => t.TransactionNumber)
                .IsUnique();

            builder.HasIndex(t => new { t.AccountId, t.TransactionDate });
            builder.HasIndex(t => new { t.UserId, t.Status });

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2);

            builder.Property(t => t.Fee)
                .HasPrecision(18, 2);

            builder.Property(t => t.BalanceAfter)
                .HasPrecision(18, 2);

            // Relationships
            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Payee)
                .WithMany(p => p.Transactions)
                .HasForeignKey(t => t.PayeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.Goal)
                .WithMany(g => g.Transactions)
                .HasForeignKey(t => t.GoalId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.RecurringTransaction)
                .WithMany(rt => rt.Transactions)
                .HasForeignKey(t => t.RecurringTransactionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
