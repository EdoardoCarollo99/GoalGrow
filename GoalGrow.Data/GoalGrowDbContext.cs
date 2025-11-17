using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Models;
using GoalGrow.Entity.Super;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Data
{
    public class GoalGrowDbContext : DbContext
    {
        public GoalGrowDbContext(DbContextOptions<GoalGrowDbContext> options) : base(options)
        {
        }

        // Users
        public DbSet<User> Users { get; set; }
        public DbSet<InversotorUser> InvestorUsers { get; set; }
        public DbSet<ConsultantUser> ConsultantUsers { get; set; }

        // Financial Core
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Goal> Goals { get; set; }

        // Investment System
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<InvestmentProduct> InvestmentProducts { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<FundMovement> FundMovements { get; set; }
        public DbSet<UserConsultantRelationship> UserConsultantRelationships { get; set; }
        public DbSet<CommissionTransaction> CommissionTransactions { get; set; }
        public DbSet<RiskProfile> RiskProfiles { get; set; }
        public DbSet<PortfolioSnapshot> PortfolioSnapshots { get; set; }

        // Gamification
        public DbSet<Badge> Badges { get; set; }
        public DbSet<UserBadge> UserBadges { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<UserChallenge> UserChallenges { get; set; }
        public DbSet<UserLevel> UserLevels { get; set; }

        // Notifications
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(GoalGrowDbContext).Assembly);

            // User Inheritance (TPH - Table Per Hierarchy)
            modelBuilder.Entity<User>()
                .HasDiscriminator<UserType>("UserType")
                .HasValue<User>(UserType.AdminUser)
                .HasValue<InversotorUser>(UserType.InvestorUser)
                .HasValue<ConsultantUser>(UserType.ConsultantUser);
        }
    }
}
