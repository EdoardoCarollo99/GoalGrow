using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Data.Contexts
{
    /// <summary>
    /// DbContext per il modulo investimenti (Portfolio, Investment, RiskProfile, ecc.)
    /// </summary>
    public class InvestmentDbContext : DbContext
    {
        public InvestmentDbContext(DbContextOptions<InvestmentDbContext> options) : base(options)
        {
        }

        // Investment System Entities
        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<InvestmentProduct> InvestmentProducts { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<CompanyAccount> CompanyAccounts { get; set; }
        public DbSet<FundMovement> FundMovements { get; set; }
        public DbSet<UserConsultantRelationship> UserConsultantRelationships { get; set; }
        public DbSet<CommissionTransaction> CommissionTransactions { get; set; }
        public DbSet<RiskProfile> RiskProfiles { get; set; }
        public DbSet<PortfolioSnapshot> PortfolioSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations for Investment module
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(InvestmentDbContext).Assembly,
                t => t.Namespace?.Contains("Configurations.Investment") ?? false
            );
        }
    }
}
