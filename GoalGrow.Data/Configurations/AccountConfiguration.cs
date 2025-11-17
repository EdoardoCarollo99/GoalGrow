using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.AccountNumber);
            builder.HasIndex(a => new { a.UserId, a.Status });

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2);

            builder.Property(a => a.AvailableBalance)
                .HasPrecision(18, 2);

            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
