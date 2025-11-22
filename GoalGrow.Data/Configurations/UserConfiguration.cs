using GoalGrow.Entity.Super;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoalGrow.Data.Configurations
{
    /// <summary>
    /// Configurazione base per la tabella Users (TPH - Table Per Hierarchy)
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            // Indici per query frequenti
            builder.HasIndex(u => u.EmailAddress)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.PhoneNumber)
                .HasDatabaseName("IX_Users_PhoneNumber");

            builder.HasIndex(u => u.UserType)
                .HasDatabaseName("IX_Users_UserType");

            // Constraints
            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.EmailAddress)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.UserType)
                .IsRequired();

            // Discriminator configurato in GoalGrowDbContext.OnModelCreating
        }
    }
}
