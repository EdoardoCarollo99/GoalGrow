using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class InvestmentProductConfiguration : IEntityTypeConfiguration<InvestmentProduct>
    {
        public void Configure(EntityTypeBuilder<InvestmentProduct> builder)
        {
            builder.HasKey(ip => ip.Id);

            builder.HasIndex(ip => ip.Code).IsUnique();
            builder.HasIndex(ip => ip.ISIN);

            builder.Property(ip => ip.CurrentPrice).HasPrecision(18, 2);
            builder.Property(ip => ip.MinimumInvestment).HasPrecision(18, 2);
            builder.Property(ip => ip.YearlyReturn).HasPrecision(18, 4);
            builder.Property(ip => ip.ExpectedReturn).HasPrecision(18, 4);
        }
    }
}
