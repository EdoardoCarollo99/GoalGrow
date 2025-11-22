using GoalGrow.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace GoalGrow.Migration
{
    public class Program
    {
        private static IConfiguration? _configuration;
        private static string? _connectionString;
        private static string? _masterConnectionString;

        public static async Task Main(string[] args)
        {
            PrintHeader();

            try
            {
                LoadConfiguration();
                
                if (string.IsNullOrEmpty(_connectionString))
                {
                    PrintConfigurationHelp();
                    return;
                }

                bool running = true;
                while (running)
                {
                    PrintMenu();
                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await ResetDatabaseAsync();
                            break;
                        case "2":
                            await CreateDatabaseAsync();
                            break;
                        case "3":
                            await ApplyMigrationsAsync();
                            break;
                        case "4":
                            await SeedDatabaseAsync();
                            break;
                        case "5":
                            await FullSetupAsync();
                            break;
                        case "6":
                            await DropDatabaseAsync();
                            break;
                        case "7":
                            await ShowDatabaseInfoAsync();
                            break;
                        case "0":
                            running = false;
                            Console.WriteLine("\nArrivederci!");
                            break;
                        default:
                            Console.WriteLine("\nOpzione non valida!");
                            break;
                    }

                    if (running && choice != "0")
                    {
                        Console.WriteLine("\nPremi un tasto per continuare...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nERRORE CRITICO: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.ResetColor();
                Console.WriteLine("\nPremi un tasto per uscire...");
                Console.ReadKey();
            }
        }

        private static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("    GoalGrow - Database Migration Tool");
            Console.WriteLine("    .NET 10 | EF Core 10 | SQL Server");
            Console.WriteLine("===============================================");
            Console.ResetColor();
            Console.WriteLine();
        }

        private static void PrintMenu()
        {
            Console.Clear();
            PrintHeader();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database: " + (_connectionString != null ? "GoalGrowDb (Configurato)" : "Non configurato"));
            Console.ResetColor();
            Console.WriteLine();

            Console.WriteLine("MENU OPERAZIONI:");
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("  1. Reset Database Completo (Drop + Create + Migrate + Seed)");
            Console.WriteLine("  2. Crea Database");
            Console.WriteLine("  3. Applica Migration");
            Console.WriteLine("  4. Popola Database (Seeding)");
            Console.WriteLine("  5. Setup Completo (Create + Migrate + Seed)");
            Console.WriteLine("  6. Elimina Database");
            Console.WriteLine("  7. Mostra Info Database");
            Console.WriteLine("  0. Esci");
            Console.WriteLine("-----------------------------------------------");
            Console.Write("\nScelta: ");
        }

        private static void LoadConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<Program>()
                .Build();

            _connectionString = _configuration.GetConnectionString("GoalGrowDb");

            if (!string.IsNullOrEmpty(_connectionString))
            {
                var builder = new SqlConnectionStringBuilder(_connectionString)
                {
                    InitialCatalog = "master"
                };
                _masterConnectionString = builder.ConnectionString;
            }
        }

        private static void PrintConfigurationHelp()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("CONFIGURAZIONE MANCANTE!");
            Console.ResetColor();
            Console.WriteLine("\nConfigura User Secrets con:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("dotnet user-secrets set \"ConnectionStrings:GoalGrowDb\" \"Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True\"");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Premi un tasto per uscire...");
            Console.ReadKey();
        }

        #region Database Operations

        private static async Task FullSetupAsync()
        {
            Console.WriteLine("\n===============================================");
            Console.WriteLine("         SETUP COMPLETO DATABASE");
            Console.WriteLine("===============================================\n");

            await CreateDatabaseAsync();
            await ApplyMigrationsAsync();
            await SeedDatabaseAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nSETUP COMPLETATO CON SUCCESSO!");
            Console.ResetColor();
        }

        private static async Task ResetDatabaseAsync()
        {
            Console.WriteLine("\n===============================================");
            Console.WriteLine("         RESET DATABASE COMPLETO");
            Console.WriteLine("===============================================\n");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("ATTENZIONE: Tutti i dati saranno eliminati! Confermi? (S/N): ");
            Console.ResetColor();
            
            if (Console.ReadLine()?.ToUpper() != "S")
            {
                Console.WriteLine("Operazione annullata.");
                return;
            }

            await DropDatabaseAsync();
            await CreateDatabaseAsync();
            await ApplyMigrationsAsync();
            await SeedDatabaseAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nRESET COMPLETATO CON SUCCESSO!");
            Console.ResetColor();
        }

        private static async Task CreateDatabaseAsync()
        {
            Console.WriteLine("\n[1/3] Creazione Database...");

            using (var connection = new SqlConnection(_masterConnectionString))
            {
                await connection.OpenAsync();

                var checkDbCmd = connection.CreateCommand();
                checkDbCmd.CommandText = "SELECT database_id FROM sys.databases WHERE name = 'GoalGrowDb'";
                var exists = await checkDbCmd.ExecuteScalarAsync() != null;

                if (exists)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  Database gia' esistente");
                    Console.ResetColor();
                    return;
                }

                var createCmd = connection.CreateCommand();
                createCmd.CommandText = "CREATE DATABASE GoalGrowDb";
                await createCmd.ExecuteNonQueryAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  [OK] Database creato");
                Console.ResetColor();
            }
        }

        private static async Task ApplyMigrationsAsync()
        {
            Console.WriteLine("\n[2/3] Applicazione Migration...");

            var optionsBuilder = new DbContextOptionsBuilder<GoalGrowDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);

            using var context = new GoalGrowDbContext(optionsBuilder.Options);
            await context.Database.MigrateAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  [OK] Migration applicate");
            Console.ResetColor();
        }

        private static async Task SeedDatabaseAsync()
        {
            Console.WriteLine("\n[3/3] Popolamento Database...\n");

            var optionsBuilder = new DbContextOptionsBuilder<GoalGrowDbContext>();
            optionsBuilder.UseSqlServer(_connectionString);

            using var context = new GoalGrowDbContext(optionsBuilder.Options);
            var seeder = new DatabaseSeeder(context);
            await seeder.SeedAsync();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  [OK] Database popolato");
            Console.ResetColor();
        }

        private static async Task DropDatabaseAsync()
        {
            Console.WriteLine("\n[DROP] Eliminazione Database...");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Confermi eliminazione database? (S/N): ");
            Console.ResetColor();

            if (Console.ReadLine()?.ToUpper() != "S")
            {
                Console.WriteLine("Operazione annullata.");
                return;
            }

            using (var connection = new SqlConnection(_masterConnectionString))
            {
                await connection.OpenAsync();

                var killCmd = connection.CreateCommand();
                killCmd.CommandText = @"
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = 'GoalGrowDb')
                    BEGIN
                        ALTER DATABASE GoalGrowDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE GoalGrowDb;
                    END";
                await killCmd.ExecuteNonQueryAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  [OK] Database eliminato");
                Console.ResetColor();
            }
        }

        private static async Task ShowDatabaseInfoAsync()
        {
            Console.WriteLine("\n===============================================");
            Console.WriteLine("         INFORMAZIONI DATABASE");
            Console.WriteLine("===============================================\n");

            try
            {
                using (var connection = new SqlConnection(_masterConnectionString))
                {
                    await connection.OpenAsync();

                    var checkCmd = connection.CreateCommand();
                    checkCmd.CommandText = @"
                        SELECT 
                            name,
                            state_desc,
                            recovery_model_desc,
                            (SELECT SUM(size) * 8 / 1024 FROM sys.master_files WHERE database_id = d.database_id) AS SizeMB
                        FROM sys.databases d
                        WHERE name = 'GoalGrowDb'";

                    using var reader = await checkCmd.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        Console.WriteLine($"Nome:            {reader["name"]}");
                        Console.WriteLine($"Stato:           {reader["state_desc"]}");
                        Console.WriteLine($"Recovery Model:  {reader["recovery_model_desc"]}");
                        Console.WriteLine($"Dimensione:      {reader["SizeMB"]} MB");

                        reader.Close();

                        var tableCmd = connection.CreateCommand();
                        tableCmd.CommandText = "USE GoalGrowDb; SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
                        var tableCount = await tableCmd.ExecuteScalarAsync();
                        Console.WriteLine($"Tabelle:         {tableCount}");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\n[OK] Database operativo");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Database non trovato.");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Errore: {ex.Message}");
                Console.ResetColor();
            }
        }

        #endregion
    }
}
