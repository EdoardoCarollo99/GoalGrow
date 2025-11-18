using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Data.Contexts
{
    /// <summary>
    /// DbContext per il modulo finanziario (Account, Transaction, Budget, Goal)
    /// </summary>
    public class FinancialDbContext : DbContext
    {
        public FinancialDbContext(DbContextOptions<FinancialDbContext> options) : base(options)
        {
        }

        // Financial Core Entities
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }
        public DbSet<Payee> Payees { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations for Financial module
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(FinancialDbContext).Assembly,
                t => t.Namespace?.Contains("Configurations.Financial") ?? false
            );
        }
    }
}
