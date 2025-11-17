using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class PayeeConfiguration : IEntityTypeConfiguration<Payee>
    {
        public void Configure(EntityTypeBuilder<Payee> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasIndex(p => p.UserId);

            // Relationship
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
