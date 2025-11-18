using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations.Compliance
{
    public class KycVerificationConfiguration : IEntityTypeConfiguration<KycVerification>
    {
        public void Configure(EntityTypeBuilder<KycVerification> builder)
        {
            builder.ToTable("KycVerifications");

            builder.HasKey(k => k.Id);

            // Indexes for performance
            builder.HasIndex(k => k.UserId)
                .IsUnique()
                .HasDatabaseName("IX_KycVerifications_UserId");

            builder.HasIndex(k => k.Status)
                .HasDatabaseName("IX_KycVerifications_Status");

            builder.HasIndex(k => new { k.Status, k.SubmittedAt })
                .HasDatabaseName("IX_KycVerifications_Status_SubmittedAt");

            builder.HasIndex(k => k.ExternalProviderId)
                .HasDatabaseName("IX_KycVerifications_ExternalProviderId");

            // Required fields
            builder.Property(k => k.Status)
                .IsRequired();

            builder.Property(k => k.DocumentType)
                .HasMaxLength(50);

            builder.Property(k => k.DocumentNumber)
                .HasMaxLength(100);

            builder.Property(k => k.DocumentFrontImageUrl)
                .HasMaxLength(500);

            builder.Property(k => k.DocumentBackImageUrl)
                .HasMaxLength(500);

            builder.Property(k => k.SelfieImageUrl)
                .HasMaxLength(500);

            builder.Property(k => k.ProofOfAddressImageUrl)
                .HasMaxLength(500);

            builder.Property(k => k.RejectionReason)
                .HasMaxLength(1000);

            builder.Property(k => k.VerificationMethod)
                .HasMaxLength(100);

            builder.Property(k => k.ExternalProviderId)
                .HasMaxLength(200);

            builder.Property(k => k.ExternalReferenceId)
                .HasMaxLength(100);

            builder.Property(k => k.RiskScore)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(k => k.RiskLevel)
                .HasMaxLength(50)
                .HasDefaultValue("Unknown");

            builder.Property(k => k.PepDetails)
                .HasMaxLength(1000);

            builder.Property(k => k.SanctionsDetails)
                .HasMaxLength(1000);

            // Relationships
            // KYC is only for Investor Users, relationship configured from User side
            builder.HasOne(k => k.User)
                .WithMany()
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(k => k.VerifiedBy)
                .WithMany()
                .HasForeignKey(k => k.VerifiedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Audit fields (from AuditableEntity)
            builder.Property(k => k.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(k => k.UpdatedAt)
                .IsRequired(false);

            // Computed columns excluded from DB
            builder.Ignore(k => k.IsVerified);
            builder.Ignore(k => k.IsRejected);
            builder.Ignore(k => k.IsPending);
            builder.Ignore(k => k.DaysSinceSubmission);
        }
    }
}
