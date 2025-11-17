using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class UserConsultantRelationshipConfiguration : IEntityTypeConfiguration<UserConsultantRelationship>
    {
        public void Configure(EntityTypeBuilder<UserConsultantRelationship> builder)
        {
            builder.HasKey(ucr => ucr.Id);

            builder.HasIndex(ucr => ucr.InvestorUserId);
            builder.HasIndex(ucr => new { ucr.ConsultantUserId, ucr.Status });

            // Relazioni
            builder.HasOne(ucr => ucr.InvestorUser)
                .WithOne(iu => iu.ConsultantRelationship)
                .HasForeignKey<UserConsultantRelationship>(ucr => ucr.InvestorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ucr => ucr.ConsultantUser)
                .WithMany(cu => cu.Clients)
                .HasForeignKey(ucr => ucr.ConsultantUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
