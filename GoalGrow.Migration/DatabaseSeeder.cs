using GoalGrow.Data;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Migration
{
    /// <summary>
    /// Seeder essenziale per inizializzare il database con dati minimi necessari
    /// </summary>
    public class DatabaseSeeder
    {
        private readonly GoalGrowDbContext _context;
        private Guid _companyAccountId;
        private AdminUser _adminUser = null!;
        private ConsultantUser _consultantUser = null!;
        private InversotorUser _investorUser = null!;

        public DatabaseSeeder(GoalGrowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                Console.WriteLine("? Database già popolato. Skip seeding.");
                return;
            }

            Console.WriteLine("?? Inizio seeding del database...\n");

            await SeedCompanyAccountAsync();
            await SeedUsersAsync(); // Admin, Consultant, Investor
            await SeedEssentialBadgesAsync(); // Solo badge fondamentali
            await SeedEssentialChallengesAsync(); // Solo sfide base
            await SeedEssentialProductsAsync(); // Pochi prodotti chiave
            
            await _context.SaveChangesAsync();

            Console.WriteLine("\n? Seeding completato!");
            await PrintStatisticsAsync();
        }

        private async Task SeedCompanyAccountAsync()
        {
            Console.Write("?? Creazione conto aziendale... ");
            
            var companyAccount = new CompanyAccount(
                accountName: "GoalGrow Master Account",
                accountNumber: "IT60X0542811101000000999999"
            );
            companyAccount.IBAN = "IT60X0542811101000000999999";
            companyAccount.BankName = "Banca Intesa Sanpaolo";
            companyAccount.BankCode = "03069";
            companyAccount.Balance = 1000000m; // Capitale iniziale
            companyAccount.AvailableBalance = 1000000m;

            _context.CompanyAccounts.Add(companyAccount);
            await _context.SaveChangesAsync();
            
            _companyAccountId = companyAccount.Id;
            Console.WriteLine("?");
        }

        private async Task SeedUsersAsync()
        {
            Console.WriteLine("?? Creazione utenti di sistema...");

            // 1. Admin User (SuperAdmin)
            Console.Write("  ?? Admin... ");
            _adminUser = new AdminUser(
                firstName: "System",
                lastName: "Administrator",
                phoneNumber: "+39 340 0000001",
                emailAddress: "admin@goalgrow.com",
                role: "SuperAdmin"
            );
            _adminUser.Permissions = "all";
            _adminUser.Department = "IT";
            _adminUser.HireDate = DateTime.UtcNow.AddMonths(-6);
            _adminUser.Notes = "Account amministratore di sistema";
            
            _context.AdminUsers.Add(_adminUser);
            Console.WriteLine("?");

            // 2. Consultant User (Esempio)
            Console.Write("  ?? Consultant... ");
            _consultantUser = new ConsultantUser(
                firstName: "Laura",
                lastName: "Bianchi",
                phoneNumber: "+39 340 1111111",
                emailAddress: "consultant@goalgrow.com",
                licenseNumber: "OCF-TEST-001"
            );
            _consultantUser.Specialization = "Investimenti Diversificati";
            _consultantUser.CommissionRate = 1.5m;
            _consultantUser.YearsOfExperience = 10;
            _consultantUser.Biography = "Consulente finanziario certificato con 10 anni di esperienza.";
            _consultantUser.AverageRating = 5.0m;
            _consultantUser.TotalReviews = 0;
            _consultantUser.CertificationDate = DateTime.UtcNow.AddYears(-10);
            
            _context.ConsultantUsers.Add(_consultantUser);
            Console.WriteLine("?");

            // 3. Investor User (Esempio)
            Console.Write("  ?? Investor... ");
            _investorUser = new InversotorUser(
                firstName: "Mario",
                lastName: "Rossi",
                phoneNumber: "+39 340 2222222",
                emailAddress: "investor@goalgrow.com",
                fiscalCode: "RSSMRA85M01H501X",
                birthDate: new DateTime(1985, 08, 01)
            );
            _investorUser.VirtualWalletBalance = 0m;
            _investorUser.TotalDeposited = 0m;
            _investorUser.TotalInvested = 0m;
            
            _context.InvestorUsers.Add(_investorUser);
            await _context.SaveChangesAsync(); // Save per avere gli Id
            Console.WriteLine("?");

            // Crea entità correlate per Investor
            await CreateInvestorRelatedEntitiesAsync(_investorUser);
        }

        private async Task CreateInvestorRelatedEntitiesAsync(InversotorUser investor)
        {
            // KYC Verification (Pending)
            var kyc = new KycVerification
            {
                UserId = investor.Id,
                Status = KycStatus.Pending,
                DocumentType = "ID",
                DocumentNumber = investor.FiscalCode,
                SubmittedAt = DateTime.UtcNow,
                RiskScore = 0,
                RiskLevel = "Unverified",
                IsPoliticallyExposed = false,
                IsOnSanctionsList = false
            };
            _context.KycVerifications.Add(kyc);

            // UserLevel (Livello 1)
            var level = new UserLevel(investor.Id, 1, "Beginner");
            _context.UserLevels.Add(level);

            // RiskProfile (Default conservativo)
            var riskProfile = new RiskProfile(
                investor.Id,
                RiskTolerance.Moderate,
                InvestmentExperience.Beginner
            );
            riskProfile.InvestmentObjective = InvestmentObjective.BalancedGrowth;
            riskProfile.InvestmentHorizon = InvestmentHorizon.Medium;
            riskProfile.RiskScore = 50;
            _context.RiskProfiles.Add(riskProfile);

            // Account (Conto Corrente)
            var account = new Account(
                accountNumber: $"IT60X054281110100000{investor.Id.ToString()[..6]}",
                accountName: "Conto Principale",
                type: AccountType.Checking,
                userId: investor.Id
            );
            account.BankName = "Intesa Sanpaolo";
            account.Balance = 0m;
            account.AvailableBalance = 0m;
            _context.Accounts.Add(account);

            // Goal di sistema: Emergency Fund
            var emergencyGoal = Goal.CreateEmergencyGoal(investor.Id, 3000m);
            _context.Goals.Add(emergencyGoal);

            // Goal di sistema: Investment Fund
            var investmentGoal = Goal.CreateInvestmentGoal(investor.Id, 5000m);
            _context.Goals.Add(investmentGoal);

            await _context.SaveChangesAsync();
        }

        private async Task SeedEssentialBadgesAsync()
        {
            Console.Write("?? Creazione badge essenziali... ");
            
            var badges = new List<Badge>
            {
                new Badge("FIRST_DEPOSIT", "Primo Deposito", "Hai depositato fondi per la prima volta!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Saving,
                    PointsReward = 25,
                    IconUrl = "/badges/first_deposit.png"
                },
                new Badge("FIRST_GOAL", "Primo Obiettivo", "Hai creato il tuo primo obiettivo!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Goal,
                    PointsReward = 10,
                    IconUrl = "/badges/first_goal.png"
                },
                new Badge("FIRST_INVESTMENT", "Primo Investimento", "Hai effettuato il tuo primo investimento!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 50,
                    IconUrl = "/badges/first_investment.png"
                },
                new Badge("KYC_VERIFIED", "Account Verificato", "Hai completato la verifica KYC!", BadgeRarity.Uncommon)
                {
                    Category = BadgeCategory.Milestone,
                    PointsReward = 100,
                    IconUrl = "/badges/kyc_verified.png"
                }
            };

            _context.Badges.AddRange(badges);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({badges.Count} badge)");
        }

        private async Task SeedEssentialChallengesAsync()
        {
            Console.Write("?? Creazione sfide base... ");
            
            var challenges = new List<Challenge>
            {
                new Challenge(
                    "?? Risparmia 500€",
                    "Deposita almeno 500€ nel tuo wallet entro 30 giorni",
                    ChallengeType.Saving,
                    DateTime.UtcNow.AddDays(30)
                )
                {
                    Difficulty = ChallengeDifficulty.Easy,
                    PointsReward = 100,
                    MoneyReward = 10m,
                    IconUrl = "/challenges/save_500.png"
                },
                new Challenge(
                    "?? Primo Investimento",
                    "Effettua il tuo primo investimento",
                    ChallengeType.Investment,
                    DateTime.UtcNow.AddDays(60)
                )
                {
                    Difficulty = ChallengeDifficulty.Easy,
                    PointsReward = 200,
                    MoneyReward = 25m,
                    IconUrl = "/challenges/first_investment.png"
                }
            };

            _context.Challenges.AddRange(challenges);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({challenges.Count} sfide)");
        }

        private async Task SeedEssentialProductsAsync()
        {
            Console.Write("?? Creazione prodotti finanziari essenziali... ");
            
            var products = new List<InvestmentProduct>
            {
                // ETF Globale
                new InvestmentProduct("VWCE", "Vanguard FTSE All-World UCITS ETF", InvestmentProductType.ETF, 105.30m)
                {
                    ISIN = "IE00BK5BQT80",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 50m,
                    YearlyReturn = 8.5m,
                    ExpectedReturn = 7.0m,
                    Issuer = "Vanguard",
                    Description = "ETF globale diversificato su 3.900+ azioni"
                },
                // Azione Tech
                new InvestmentProduct("AAPL", "Apple Inc.", InvestmentProductType.Stock, 178.50m)
                {
                    ISIN = "US0378331005",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 100m,
                    YearlyReturn = 12.5m,
                    Issuer = "Apple Inc.",
                    Description = "Leader mondiale nella tecnologia consumer"
                },
                // Obbligazione Sicura
                new InvestmentProduct("BTP-10Y", "BTP Italia 10 anni", InvestmentProductType.Bond, 98.50m)
                {
                    ISIN = "IT0005441883",
                    RiskLevel = RiskLevel.Low,
                    MinimumInvestment = 1000m,
                    YearlyReturn = 3.8m,
                    ExpectedReturn = 3.5m,
                    Issuer = "Repubblica Italiana",
                    Description = "Buoni del Tesoro Poliennali Italia"
                }
            };

            _context.InvestmentProducts.AddRange(products);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({products.Count} prodotti)");
        }

        private async Task PrintStatisticsAsync()
        {
            var usersCount = await _context.Users.CountAsync();
            var adminCount = await _context.AdminUsers.CountAsync();
            var consultantsCount = await _context.ConsultantUsers.CountAsync();
            var investorsCount = await _context.InvestorUsers.CountAsync();
            var productsCount = await _context.InvestmentProducts.CountAsync();
            var badgesCount = await _context.Badges.CountAsync();
            var challengesCount = await _context.Challenges.CountAsync();
            var goalsCount = await _context.Goals.CountAsync();

            Console.WriteLine($"\n?? Statistiche Database:");
            Console.WriteLine($"  ?? Utenti totali: {usersCount}");
            Console.WriteLine($"  ?? Admin: {adminCount}");
            Console.WriteLine($"  ?? Consulenti: {consultantsCount}");
            Console.WriteLine($"  ?? Investitori: {investorsCount}");
            Console.WriteLine($"  ?? Prodotti finanziari: {productsCount}");
            Console.WriteLine($"  ?? Badge: {badgesCount}");
            Console.WriteLine($"  ?? Sfide: {challengesCount}");
            Console.WriteLine($"  ?? Goals: {goalsCount}");
            Console.WriteLine($"\n? Sistema pronto per l'uso!");
        }
    }
}
