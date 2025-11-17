using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class CompanyAccountConfiguration : IEntityTypeConfiguration<CompanyAccount>
    {
        public void Configure(EntityTypeBuilder<CompanyAccount> builder)
        {
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.Balance).HasPrecision(18, 2);
            builder.Property(ca => ca.AvailableBalance).HasPrecision(18, 2);
            builder.Property(ca => ca.TotalDeposits).HasPrecision(18, 2);
            builder.Property(ca => ca.TotalWithdrawals).HasPrecision(18, 2);
            builder.Property(ca => ca.TotalInvested).HasPrecision(18, 2);
        }
    }
}
