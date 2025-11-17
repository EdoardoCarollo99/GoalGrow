using GoalGrow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GoalGrow.Migration
{
    /// <summary>
    /// Factory per la creazione del DbContext durante le migration.
    /// Legge la connection string da User Secrets.
    /// </summary>
    public class GoalGrowDbContextFactory : IDesignTimeDbContextFactory<GoalGrowDbContext>
    {
        public GoalGrowDbContext CreateDbContext(string[] args)
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<GoalGrowDbContextFactory>()
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("GoalGrowDb");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'GoalGrowDb' non trovata. " +
                    "Configura User Secrets con: dotnet user-secrets set \"ConnectionStrings:GoalGrowDb\" \"your-connection-string\"");
            }

            // Build DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<GoalGrowDbContext>();
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            return new GoalGrowDbContext(optionsBuilder.Options);
        }
    }
}
