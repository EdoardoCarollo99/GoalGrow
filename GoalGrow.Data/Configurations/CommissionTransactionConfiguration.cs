using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class CommissionTransactionConfiguration : IEntityTypeConfiguration<CommissionTransaction>
    {
        public void Configure(EntityTypeBuilder<CommissionTransaction> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.CommissionNumber).IsUnique();
            builder.HasIndex(c => new { c.ConsultantId, c.Status });

            builder.Property(c => c.Amount).HasPrecision(18, 2);
            builder.Property(c => c.BaseAmount).HasPrecision(18, 2);
            builder.Property(c => c.CommissionRate).HasPrecision(18, 4);

            // Relationships con NoAction per evitare cascade cycles
            builder.HasOne(c => c.Consultant)
                .WithMany(cu => cu.Commissions)
                .HasForeignKey(c => c.ConsultantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.InvestorUser)
                .WithMany(iu => iu.CommissionTransactions)
                .HasForeignKey(c => c.InvestorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Investment)
                .WithMany()
                .HasForeignKey(c => c.InvestmentId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
