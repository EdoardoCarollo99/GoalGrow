using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.Property(a => a.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Permissions)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.Department)
                .HasMaxLength(100);

            builder.Property(a => a.Notes)
                .HasMaxLength(1000);

            builder.Property(a => a.IsSuperAdmin)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
