using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations.Financial
{
    public class PlatformFeeConfiguration : IEntityTypeConfiguration<PlatformFee>
    {
        public void Configure(EntityTypeBuilder<PlatformFee> builder)
        {
            builder.ToTable("PlatformFees");

            builder.HasKey(p => p.Id);

            // Indexes for performance
            builder.HasIndex(p => p.UserId)
                .HasDatabaseName("IX_PlatformFees_UserId");

            builder.HasIndex(p => p.FeeNumber)
                .IsUnique()
                .HasDatabaseName("IX_PlatformFees_FeeNumber");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_PlatformFees_Status");

            builder.HasIndex(p => new { p.UserId, p.TransactionDate })
                .HasDatabaseName("IX_PlatformFees_UserId_TransactionDate");

            builder.HasIndex(p => new { p.Type, p.Status })
                .HasDatabaseName("IX_PlatformFees_Type_Status");

            builder.HasIndex(p => p.RelatedTransactionId)
                .HasDatabaseName("IX_PlatformFees_RelatedTransactionId");

            builder.HasIndex(p => p.RelatedInvestmentId)
                .HasDatabaseName("IX_PlatformFees_RelatedInvestmentId");

            builder.HasIndex(p => p.RelatedFundMovementId)
                .HasDatabaseName("IX_PlatformFees_RelatedFundMovementId");

            // Required fields
            builder.Property(p => p.FeeNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.Property(p => p.Type)
                .IsRequired();

            builder.Property(p => p.BaseAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            builder.Property(p => p.FeePercentage)
                .IsRequired()
                .HasColumnType("decimal(18,4)")
                .HasDefaultValue(1.00m); // Default 1%

            builder.Property(p => p.MinimumFee)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(1.00m); // Default €1

            builder.Property(p => p.CalculatedFee)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m);

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("EUR");

            builder.Property(p => p.TransactionDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.Notes)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.RelatedTransaction)
                .WithMany()
                .HasForeignKey(p => p.RelatedTransactionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.RelatedInvestment)
                .WithMany()
                .HasForeignKey(p => p.RelatedInvestmentId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(p => p.RelatedFundMovement)
                .WithMany()
                .HasForeignKey(p => p.RelatedFundMovementId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Audit fields (from AuditableEntity)
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            // Computed columns excluded from DB
            builder.Ignore(p => p.IsCollected);
        }
    }
}
