using GoalGrow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GoalGrow.Migration
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== GoalGrow Database Migration Tool ===\n");

            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddUserSecrets<Program>()
                    .Build();

                var connectionString = configuration.GetConnectionString("GoalGrowDb");

                if (string.IsNullOrEmpty(connectionString))
                {
                    Console.WriteLine("❌ Connection string non trovata!");
                    Console.WriteLine("\nConfigura User Secrets con:");
                    Console.WriteLine("dotnet user-secrets set \"ConnectionStrings:GoalGrowDb\" \"Server=(localdb)\\\\mssqllocaldb;Database=GoalGrowDb;Trusted_Connection=true;TrustServerCertificate=true;\"");
                    return;
                }

                Console.WriteLine("✓ Connection string caricata\n");

                var optionsBuilder = new DbContextOptionsBuilder<GoalGrowDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                using var context = new GoalGrowDbContext(optionsBuilder.Options);

                Console.WriteLine("Scegli un'opzione:");
                Console.WriteLine("1. Crea database e applica migration");
                Console.WriteLine("2. Popola database con dati di esempio");
                Console.WriteLine("3. Elimina database");
                Console.WriteLine("4. Esci");
                Console.Write("\nScelta: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await CreateDatabaseAsync(context);
                        break;
                    case "2":
                        await SeedDatabaseAsync(context);
                        break;
                    case "3":
                        await DeleteDatabaseAsync(context);
                        break;
                    case "4":
                        Console.WriteLine("Uscita...");
                        break;
                    default:
                        Console.WriteLine("Opzione non valida");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Errore: {ex.Message}");
            }

            Console.WriteLine("\nPremi un tasto per uscire...");
            Console.ReadKey();
        }

        private static async Task CreateDatabaseAsync(GoalGrowDbContext context)
        {
            Console.WriteLine("\n📦 Creazione database...");
            await context.Database.EnsureDeletedAsync();
            Console.WriteLine("✓ Database esistente eliminato");

            await context.Database.EnsureCreatedAsync();
            Console.WriteLine("✓ Database creato!");
        }

        private static async Task SeedDatabaseAsync(GoalGrowDbContext context)
        {
            Console.WriteLine("\n🌱 Popolamento database...");
            var seeder = new DatabaseSeeder(context);
            await seeder.SeedAsync();
            Console.WriteLine("\n✅ Database popolato!");
        }

        private static async Task DeleteDatabaseAsync(GoalGrowDbContext context)
        {
            Console.Write("\n⚠️  Confermi eliminazione? (S/N): ");
            if (Console.ReadLine()?.ToUpper() == "S")
            {
                await context.Database.EnsureDeletedAsync();
                Console.WriteLine("✓ Database eliminato");
            }
        }
    }
}
