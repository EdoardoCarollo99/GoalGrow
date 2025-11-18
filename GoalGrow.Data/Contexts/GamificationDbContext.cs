using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Data.Contexts
{
    /// <summary>
    /// DbContext per il modulo gamification (Badge, Challenge, UserLevel)
    /// </summary>
    public class GamificationDbContext : DbContext
    {
        public GamificationDbContext(DbContextOptions<GamificationDbContext> options) : base(options)
        {
        }

        // Gamification Entities
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations for Gamification module
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(GamificationDbContext).Assembly,
                t => t.Namespace?.Contains("Configurations.Gamification") ?? false
            );
        }
    }
}
