using GoalGrow.Data;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Models;
using Microsoft.EntityFrameworkCore;

namespace GoalGrow.Migration
{
    public class DatabaseSeeder
    {
        private readonly GoalGrowDbContext _context;
        private Guid _companyAccountId;

        public DatabaseSeeder(GoalGrowDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                Console.WriteLine("Database già popolato. Skip seeding.");
                return;
            }

            Console.WriteLine("?? Inizio seeding del database...\n");

            await SeedCompanyAccountAsync();
            await SeedBadgesAsync();
            await SeedChallengesAsync();
            await SeedInvestmentProductsAsync();
            await SeedConsultantsAsync();
            await SeedInvestorsAsync();

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
            companyAccount.Balance = 500000m;
            companyAccount.AvailableBalance = 500000m;

            _context.CompanyAccounts.Add(companyAccount);
            await _context.SaveChangesAsync();
            
            _companyAccountId = companyAccount.Id;
            Console.WriteLine("?");
        }

        private async Task SeedBadgesAsync()
        {
            Console.Write("?? Creazione badge... ");
            
            var badges = new List<Badge>
            {
                // Common badges
                new Badge("FIRST_GOAL", "Primo Traguardo", "Hai creato il tuo primo obiettivo finanziario!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Goal,
                    PointsReward = 10,
                    IconUrl = "/badges/first_goal.png",
                    Requirements = "Crea il tuo primo obiettivo"
                },
                new Badge("FIRST_INVESTMENT", "Primo Investimento", "Hai effettuato il tuo primo investimento!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 50,
                    IconUrl = "/badges/first_investment.png",
                    Requirements = "Effettua il tuo primo investimento"
                },
                new Badge("FIRST_DEPOSIT", "Primo Deposito", "Hai depositato fondi per la prima volta!", BadgeRarity.Common)
                {
                    Category = BadgeCategory.Saving,
                    PointsReward = 25,
                    IconUrl = "/badges/first_deposit.png",
                    Requirements = "Effettua il tuo primo deposito"
                },

                // Uncommon badges
                new Badge("SAVER_500", "Risparmiatore 500", "Hai risparmiato 500€!", BadgeRarity.Uncommon)
                {
                    Category = BadgeCategory.Saving,
                    PointsReward = 100,
                    IconUrl = "/badges/saver_500.png",
                    Requirements = "Risparmia 500€"
                },
                new Badge("INVESTOR_1K", "Investitore 1K", "Hai investito 1.000€!", BadgeRarity.Uncommon)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 150,
                    IconUrl = "/badges/investor_1k.png",
                    Requirements = "Investi almeno 1.000€"
                },

                // Rare badges
                new Badge("DIVERSIFICATION_PRO", "Pro della Diversificazione", "Hai investito in 5+ prodotti diversi!", BadgeRarity.Rare)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 250,
                    IconUrl = "/badges/diversification.png",
                    Requirements = "Investi in almeno 5 prodotti finanziari diversi"
                },
                new Badge("GOAL_MASTER", "Maestro degli Obiettivi", "Hai completato 5 obiettivi!", BadgeRarity.Rare)
                {
                    Category = BadgeCategory.Goal,
                    PointsReward = 300,
                    IconUrl = "/badges/goal_master.png",
                    Requirements = "Completa 5 obiettivi finanziari"
                },

                // Epic badges
                new Badge("SAVING_MASTER", "Maestro del Risparmio", "Hai risparmiato 10.000€!", BadgeRarity.Epic)
                {
                    Category = BadgeCategory.Saving,
                    PointsReward = 500,
                    IconUrl = "/badges/saving_master.png",
                    Requirements = "Risparmia 10.000€"
                },
                new Badge("INVESTOR_10K", "Investitore Esperto", "Hai investito 10.000€!", BadgeRarity.Epic)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 750,
                    IconUrl = "/badges/investor_10k.png",
                    Requirements = "Investi almeno 10.000€"
                },

                // Legendary badge
                new Badge("LEGENDARY_INVESTOR", "Investitore Leggendario", "Hai superato 100.000€ di investimenti!", BadgeRarity.Legendary)
                {
                    Category = BadgeCategory.Investment,
                    PointsReward = 2000,
                    IconUrl = "/badges/legendary_investor.png",
                    Requirements = "Supera 100.000€ di investimenti totali"
                },

                // Milestone badges
                new Badge("ONE_YEAR", "Un Anno Insieme", "Hai usato GoalGrow per un anno!", BadgeRarity.Rare)
                {
                    Category = BadgeCategory.Milestone,
                    PointsReward = 500,
                    IconUrl = "/badges/one_year.png",
                    Requirements = "Usa l'app per 365 giorni"
                },

                // Social badges
                new Badge("REFERRAL_HERO", "Eroe dei Referral", "Hai invitato 5 amici!", BadgeRarity.Epic)
                {
                    Category = BadgeCategory.Social,
                    PointsReward = 1000,
                    IconUrl = "/badges/referral_hero.png",
                    Requirements = "Invita 5 amici che si registrano"
                }
            };

            _context.Badges.AddRange(badges);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({badges.Count} badge)");
        }

        private async Task SeedChallengesAsync()
        {
            Console.Write("?? Creazione sfide... ");
            
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
                    Requirements = "Deposita 500€ nel wallet virtuale",
                    IconUrl = "/challenges/save_500.png"
                },
                new Challenge(
                    "?? Risparmia 2.000€",
                    "Deposita almeno 2.000€ nel tuo wallet entro 60 giorni",
                    ChallengeType.Saving,
                    DateTime.UtcNow.AddDays(60)
                )
                {
                    Difficulty = ChallengeDifficulty.Medium,
                    PointsReward = 300,
                    MoneyReward = 50m,
                    Requirements = "Deposita 2.000€ nel wallet virtuale",
                    IconUrl = "/challenges/save_2k.png"
                },
                new Challenge(
                    "?? Diversifica il Portafoglio",
                    "Investi in almeno 5 prodotti finanziari diversi",
                    ChallengeType.Investment,
                    DateTime.UtcNow.AddDays(90)
                )
                {
                    Difficulty = ChallengeDifficulty.Hard,
                    PointsReward = 400,
                    Requirements = "Investi in 5+ prodotti diversi",
                    IconUrl = "/challenges/diversify.png"
                },
                new Challenge(
                    "?? 30 Giorni di Budget Perfetto",
                    "Non superare nessun budget per 30 giorni consecutivi",
                    ChallengeType.Budget,
                    DateTime.UtcNow.AddDays(30)
                )
                {
                    Difficulty = ChallengeDifficulty.Expert,
                    PointsReward = 600,
                    IsRecurring = true,
                    Requirements = "Rispetta tutti i budget per 30 giorni",
                    IconUrl = "/challenges/budget_perfect.png"
                },
                new Challenge(
                    "?? Completa 3 Obiettivi",
                    "Raggiungi 3 obiettivi finanziari",
                    ChallengeType.Goal,
                    DateTime.UtcNow.AddDays(180)
                )
                {
                    Difficulty = ChallengeDifficulty.Hard,
                    PointsReward = 500,
                    Requirements = "Completa 3 obiettivi",
                    IconUrl = "/challenges/3_goals.png"
                },
                new Challenge(
                    "?? Guadagna il 10%",
                    "Ottieni un rendimento del 10% su un investimento",
                    ChallengeType.Investment,
                    DateTime.UtcNow.AddDays(365)
                )
                {
                    Difficulty = ChallengeDifficulty.Expert,
                    PointsReward = 1000,
                    MoneyReward = 100m,
                    Requirements = "Rendimento 10% su almeno un investimento",
                    IconUrl = "/challenges/10_percent.png"
                }
            };

            _context.Challenges.AddRange(challenges);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({challenges.Count} sfide)");
        }

        private async Task SeedInvestmentProductsAsync()
        {
            Console.Write("?? Creazione prodotti finanziari... ");
            
            var products = new List<InvestmentProduct>
            {
                // ETF
                new InvestmentProduct("VWCE", "Vanguard FTSE All-World UCITS ETF", InvestmentProductType.ETF, 105.30m)
                {
                    ISIN = "IE00BK5BQT80",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 50m,
                    YearlyReturn = 8.5m,
                    ExpectedReturn = 7.0m,
                    Issuer = "Vanguard",
                    Description = "ETF che replica l'indice FTSE All-World con oltre 3.900 azioni globali"
                },
                new InvestmentProduct("CSPX", "iShares Core S&P 500 UCITS ETF", InvestmentProductType.ETF, 445.60m)
                {
                    ISIN = "IE00B5BMR087",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 100m,
                    YearlyReturn = 10.2m,
                    ExpectedReturn = 8.5m,
                    Issuer = "BlackRock",
                    Description = "ETF che replica l'indice S&P 500 delle 500 maggiori aziende USA"
                },

                // Azioni
                new InvestmentProduct("AAPL", "Apple Inc.", InvestmentProductType.Stock, 178.50m)
                {
                    ISIN = "US0378331005",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 100m,
                    YearlyReturn = 12.5m,
                    Issuer = "Apple Inc.",
                    Description = "Leader mondiale nella tecnologia consumer"
                },
                new InvestmentProduct("MSFT", "Microsoft Corporation", InvestmentProductType.Stock, 425.80m)
                {
                    ISIN = "US5949181045",
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 100m,
                    YearlyReturn = 15.2m,
                    Issuer = "Microsoft Corporation",
                    Description = "Leader nel software e cloud computing"
                },
                new InvestmentProduct("NVDA", "NVIDIA Corporation", InvestmentProductType.Stock, 495.20m)
                {
                    ISIN = "US67066G1040",
                    RiskLevel = RiskLevel.High,
                    MinimumInvestment = 100m,
                    YearlyReturn = 45.8m,
                    Issuer = "NVIDIA",
                    Description = "Leader mondiale in GPU e AI"
                },
                new InvestmentProduct("TSLA", "Tesla Inc.", InvestmentProductType.Stock, 245.30m)
                {
                    ISIN = "US88160R1014",
                    RiskLevel = RiskLevel.High,
                    MinimumInvestment = 100m,
                    YearlyReturn = 25.6m,
                    Issuer = "Tesla",
                    Description = "Leader veicoli elettrici e energia sostenibile"
                },

                // Obbligazioni
                new InvestmentProduct("BTP-10Y", "BTP Italia 10 anni", InvestmentProductType.Bond, 98.50m)
                {
                    ISIN = "IT0005441883",
                    RiskLevel = RiskLevel.Low,
                    MinimumInvestment = 1000m,
                    YearlyReturn = 3.8m,
                    ExpectedReturn = 3.5m,
                    Issuer = "Repubblica Italiana",
                    Description = "Buoni del Tesoro Poliennali Italia scadenza 10 anni"
                },

                // Crypto
                new InvestmentProduct("BTC-EUR", "Bitcoin", InvestmentProductType.Crypto, 42500.00m)
                {
                    RiskLevel = RiskLevel.VeryHigh,
                    MinimumInvestment = 10m,
                    YearlyReturn = 85.2m,
                    Issuer = "Blockchain",
                    Description = "La prima e più grande criptovaluta al mondo"
                },
                new InvestmentProduct("ETH-EUR", "Ethereum", InvestmentProductType.Crypto, 2250.00m)
                {
                    RiskLevel = RiskLevel.VeryHigh,
                    MinimumInvestment = 10m,
                    YearlyReturn = 72.5m,
                    Issuer = "Ethereum Foundation",
                    Description = "Piattaforma blockchain per smart contract e DeFi"
                },

                // Commodity
                new InvestmentProduct("GOLD-EUR", "Oro Fisico", InvestmentProductType.Commodity, 1950.00m)
                {
                    RiskLevel = RiskLevel.Low,
                    MinimumInvestment = 100m,
                    YearlyReturn = 3.5m,
                    ExpectedReturn = 2.8m,
                    Issuer = "Commodity Market",
                    Description = "Oro fisico certificato (prezzo per oncia)"
                },
                new InvestmentProduct("SILVER-EUR", "Argento Fisico", InvestmentProductType.Commodity, 23.50m)
                {
                    RiskLevel = RiskLevel.Medium,
                    MinimumInvestment = 50m,
                    YearlyReturn = 5.2m,
                    Issuer = "Commodity Market",
                    Description = "Argento fisico certificato (prezzo per oncia)"
                }
            };

            _context.InvestmentProducts.AddRange(products);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({products.Count} prodotti)");
        }

        private async Task SeedConsultantsAsync()
        {
            Console.Write("?? Creazione consulenti... ");
            
            var consultants = new List<ConsultantUser>
            {
                new ConsultantUser(
                    firstName: "Laura",
                    lastName: "Bianchi",
                    phoneNumber: "+39 340 7654321",
                    emailAddress: "laura.bianchi@goalgrow.com",
                    licenseNumber: "OCF-12345"
                )
                {
                    Specialization = "Investimenti ESG e Sostenibili",
                    CommissionRate = 1.5m,
                    YearsOfExperience = 10,
                    Biography = "Esperta in investimenti sostenibili con 10 anni di esperienza nel settore finanziario. Certificata EFPA e specializzata in portafogli ESG.",
                    AverageRating = 4.8m,
                    TotalReviews = 25,
                    CertificationDate = new DateTime(2014, 06, 15)
                },
                new ConsultantUser(
                    firstName: "Marco",
                    lastName: "Verdi",
                    phoneNumber: "+39 348 9876543",
                    emailAddress: "marco.verdi@goalgrow.com",
                    licenseNumber: "OCF-67890"
                )
                {
                    Specialization = "Trading e Investimenti ad Alto Rendimento",
                    CommissionRate = 2.0m,
                    YearsOfExperience = 15,
                    Biography = "Trader professionista con 15 anni di esperienza. Specializzato in azioni growth e criptovalute.",
                    AverageRating = 4.6m,
                    TotalReviews = 42,
                    CertificationDate = new DateTime(2009, 03, 20)
                },
                new ConsultantUser(
                    firstName: "Giulia",
                    lastName: "Rossi",
                    phoneNumber: "+39 346 1112233",
                    emailAddress: "giulia.rossi@goalgrow.com",
                    licenseNumber: "OCF-11223"
                )
                {
                    Specialization = "Pianificazione Pensionistica e Long-term",
                    CommissionRate = 1.2m,
                    YearsOfExperience = 8,
                    Biography = "Specializzata in pianificazione previdenziale e investimenti a lungo termine. Approccio conservativo e orientato alla sicurezza.",
                    AverageRating = 4.9m,
                    TotalReviews = 18,
                    CertificationDate = new DateTime(2016, 09, 10)
                }
            };

            _context.ConsultantUsers.AddRange(consultants);
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"? ({consultants.Count} consulenti)");
        }

        private async Task SeedInvestorsAsync()
        {
            Console.Write("?? Creazione investitori... ");

            var consultants = await _context.ConsultantUsers.ToListAsync();
            var laura = consultants[0];
            var marco = consultants[1];
            var giulia = consultants[2];

            // Investitore 1: Mario Rossi - Principiante
            await CreateInvestorAsync(
                "Mario", "Rossi", "+39 333 1234567", "mario.rossi@email.com",
                "RSSMRA80A01H501X", new DateTime(1980, 01, 01),
                laura, RiskTolerance.Moderate, InvestmentExperience.Beginner,
                5000m, false
            );

            // Investitore 2: Anna Ferrari - Esperta
            await CreateInvestorAsync(
                "Anna", "Ferrari", "+39 339 8765432", "anna.ferrari@email.com",
                "FRRNNA85M45H501Z", new DateTime(1985, 08, 05),
                marco, RiskTolerance.Aggressive, InvestmentExperience.Advanced,
                25000m, true
            );

            // Investitore 3: Luca Moretti - Giovane
            await CreateInvestorAsync(
                "Luca", "Moretti", "+39 345 2233445", "luca.moretti@email.com",
                "MRTLCU95C12F205W", new DateTime(1995, 03, 12),
                laura, RiskTolerance.Conservative, InvestmentExperience.Beginner,
                2000m, false
            );

            // Investitore 4: Sofia Costa - Imprenditrice
            await CreateInvestorAsync(
                "Sofia", "Costa", "+39 347 5566778", "sofia.costa@email.com",
                "CSTSFO78H41L219Y", new DateTime(1978, 06, 01),
                giulia, RiskTolerance.Moderate, InvestmentExperience.Intermediate,
                50000m, true
            );

            // Investitore 5: Francesco Romano - Pensionando
            await CreateInvestorAsync(
                "Francesco", "Romano", "+39 338 9988776", "francesco.romano@email.com",
                "RMNFNC60T15A662K", new DateTime(1960, 12, 15),
                giulia, RiskTolerance.VeryConservative, InvestmentExperience.Intermediate,
                80000m, true
            );

            await _context.SaveChangesAsync();
            Console.WriteLine("? (5 investitori)");
        }

        private async Task CreateInvestorAsync(
            string firstName, string lastName, string phone, string email,
            string fiscalCode, DateTime birthDate,
            ConsultantUser consultant, RiskTolerance riskTolerance, InvestmentExperience experience,
            decimal initialBalance, bool createInvestments)
        {
            var investor = new InversotorUser(firstName, lastName, phone, email, fiscalCode, birthDate);
            investor.VirtualWalletBalance = createInvestments ? initialBalance * 0.3m : 0m;
            investor.TotalDeposited = initialBalance;
            investor.TotalInvested = createInvestments ? initialBalance * 0.7m : 0m;

            _context.InvestorUsers.Add(investor);
            await _context.SaveChangesAsync();

            // KYC Verification
            var kycStatus = createInvestments ? KycStatus.Verified : KycStatus.Pending;
            var kyc = new KycVerification
            {
                UserId = investor.Id,
                Status = kycStatus,
                DocumentType = "ID",
                DocumentNumber = fiscalCode,
                DocumentExpiryDate = DateTime.UtcNow.AddYears(5),
                DocumentFrontImageUrl = $"/kyc/{investor.Id}/id_front.jpg",
                DocumentBackImageUrl = $"/kyc/{investor.Id}/id_back.jpg",
                SelfieImageUrl = $"/kyc/{investor.Id}/selfie.jpg",
                ProofOfAddressImageUrl = $"/kyc/{investor.Id}/utility_bill.pdf",
                SubmittedAt = DateTime.UtcNow.AddDays(-30),
                VerifiedAt = createInvestments ? DateTime.UtcNow.AddDays(-28) : null,
                VerificationMethod = createInvestments ? "Manual" : "Pending",
                RiskScore = riskTolerance == RiskTolerance.VeryConservative ? 15 : 35,
                RiskLevel = riskTolerance == RiskTolerance.VeryConservative ? "Low" : "Medium",
                IsPoliticallyExposed = false,
                IsOnSanctionsList = false
            };
            _context.KycVerifications.Add(kyc);

            // UserLevel
            var level = new UserLevel(investor.Id, 1, "Beginner");
            if (createInvestments)
            {
                level.TotalPoints = 250;
                level.CurrentLevelPoints = 50;
                level.CurrentLevel = 3;
            }
            _context.UserLevels.Add(level);

            // RiskProfile
            var riskScore = riskTolerance switch
            {
                RiskTolerance.VeryConservative => 20,
                RiskTolerance.Conservative => 35,
                RiskTolerance.Moderate => 55,
                RiskTolerance.Aggressive => 75,
                RiskTolerance.VeryAggressive => 90,
                _ => 50
            };

            var riskProfile = new RiskProfile(investor.Id, riskTolerance, experience);
            riskProfile.InvestmentObjective = riskTolerance == RiskTolerance.VeryConservative 
                ? InvestmentObjective.Preservation 
                : InvestmentObjective.BalancedGrowth;
            riskProfile.InvestmentHorizon = experience == InvestmentExperience.Beginner 
                ? InvestmentHorizon.Medium 
                : InvestmentHorizon.Long;
            riskProfile.AnnualIncome = 45000m;
            riskProfile.NetWorth = initialBalance * 3;
            riskProfile.LiquidAssets = initialBalance;
            riskProfile.RiskScore = riskScore;
            _context.RiskProfiles.Add(riskProfile);

            // Relazione consulente
            var relationship = new UserConsultantRelationship(investor.Id, consultant.Id);
            if (createInvestments)
            {
                relationship.Rating = 5;
                relationship.Review = "Ottimo consulente, molto professionale!";
                relationship.ReviewDate = DateTime.UtcNow.AddMonths(-1);
            }
            _context.UserConsultantRelationships.Add(relationship);

            // Account
            var account = new Account(
                accountNumber: $"IT60X054281110100000{investor.Id.ToString()[..6]}",
                accountName: "Conto Corrente Principale",
                type: AccountType.Checking,
                userId: investor.Id
            );
            account.BankName = "Intesa Sanpaolo";
            account.Balance = initialBalance;
            account.AvailableBalance = investor.VirtualWalletBalance;
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(); // Save account to get Id

            // Goal
            var goalAmount = experience == InvestmentExperience.Beginner ? 3000m : 10000m;
            var targetDate = DateTime.UtcNow.AddMonths(12);
            var goal = new Goal($"Obiettivo di {firstName}", goalAmount, targetDate, investor.Id);
            goal.Description = experience == InvestmentExperience.Beginner 
                ? "Risparmio per vacanza" 
                : "Fondo emergenza";
            goal.Priority = GoalPriority.High;
            goal.Category = experience == InvestmentExperience.Beginner ? "Viaggi" : "Emergenze";
            goal.CurrentAmount = createInvestments ? goalAmount * 0.4m : 0m;
            _context.Goals.Add(goal);

            // Budget
            var budget = new Budget(
                name: "Budget Mensile",
                category: "Generale",
                amount: 1000m,
                period: BudgetPeriod.Monthly,
                startDate: new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                userId: investor.Id
            );
            budget.SpentAmount = 450m;
            budget.AlertThreshold = 80;
            _context.Budgets.Add(budget);

            // Transaction (deposit)
            if (createInvestments)
            {
                var depositDate = DateTime.UtcNow.AddMonths(-3);
                var depositTx = new Transaction(
                    transactionNumber: $"TXN-{Guid.NewGuid().ToString()[..8]}",
                    type: TransactionType.Deposit,
                    transactionDate: depositDate,
                    amount: initialBalance,
                    accountId: account.Id,
                    userId: investor.Id,
                    balanceAfter: initialBalance
                );
                depositTx.Status = TransactionStatus.Completed;
                depositTx.Category = "Deposito Iniziale";
                depositTx.Description = "Deposito iniziale sul wallet virtuale";
                _context.Transactions.Add(depositTx);
                await _context.SaveChangesAsync(); // Save to get transaction Id

                // Platform Fee for deposit (1%, min €1)
                var depositFee = new PlatformFee(
                    feeNumber: $"FEE-{Guid.NewGuid().ToString()[..8]}",
                    userId: investor.Id,
                    type: PlatformFeeType.Deposit,
                    baseAmount: initialBalance,
                    feePercentage: 1.00m,
                    minimumFee: 1.00m
                );
                depositFee.RelatedTransactionId = depositTx.Id;
                depositFee.TransactionDate = depositDate;
                depositFee.Status = FeeStatus.Collected;
                depositFee.CollectedAt = depositDate.AddHours(1);
                depositFee.Description = $"Fee su deposito di €{initialBalance:N2}";
                _context.PlatformFees.Add(depositFee);
            }

            // Investimenti se richiesti
            if (createInvestments)
            {
                await CreateInvestmentsAsync(investor, consultant, account);
                
                var firstGoalBadge = await _context.Badges.FirstAsync(b => b.Code == "FIRST_GOAL");
                var firstInvestmentBadge = await _context.Badges.FirstAsync(b => b.Code == "FIRST_INVESTMENT");
                var firstDepositBadge = await _context.Badges.FirstAsync(b => b.Code == "FIRST_DEPOSIT");

                _context.UserBadges.AddRange(
                    new UserBadge(investor.Id, firstGoalBadge.Id),
                    new UserBadge(investor.Id, firstInvestmentBadge.Id),
                    new UserBadge(investor.Id, firstDepositBadge.Id)
                );
            }
        }

        private async Task CreateInvestmentsAsync(InversotorUser investor, ConsultantUser consultant, Account account)
        {
            var products = await _context.InvestmentProducts.ToListAsync();
            
            var portfolio = new Portfolio("Portafoglio Principale", investor.Id);
            portfolio.ConsultantId = consultant.Id;
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();

            var vwce = products.First(p => p.Code == "VWCE");
            var aapl = products.First(p => p.Code == "AAPL");
            
            var investment1 = new Investment(
                userId: investor.Id,
                portfolioId: portfolio.Id,
                productId: vwce.Id,
                investmentNumber: $"INV-{Guid.NewGuid().ToString()[..8]}",
                quantity: 20m,
                purchasePrice: vwce.CurrentPrice,
                totalAmount: 20m * vwce.CurrentPrice
            );
            investment1.ConsultantId = consultant.Id;
            investment1.InvestmentDate = DateTime.UtcNow.AddMonths(-3);

            var investment2 = new Investment(
                userId: investor.Id,
                portfolioId: portfolio.Id,
                productId: aapl.Id,
                investmentNumber: $"INV-{Guid.NewGuid().ToString()[..8]}",
                quantity: 10m,
                purchasePrice: aapl.CurrentPrice,
                totalAmount: 10m * aapl.CurrentPrice
            );
            investment2.ConsultantId = consultant.Id;
            investment2.InvestmentDate = DateTime.UtcNow.AddMonths(-2);

            _context.Investments.AddRange(investment1, investment2);
            await _context.SaveChangesAsync(); // Save to get investment Ids

            // Platform Fees for investments
            var fee1 = new PlatformFee(
                feeNumber: $"FEE-{Guid.NewGuid().ToString()[..8]}",
                userId: investor.Id,
                type: PlatformFeeType.Investment,
                baseAmount: investment1.TotalAmount,
                feePercentage: 1.00m,
                minimumFee: 1.00m
            );
            fee1.RelatedInvestmentId = investment1.Id;
            fee1.TransactionDate = investment1.InvestmentDate;
            fee1.Status = FeeStatus.Collected;
            fee1.CollectedAt = investment1.InvestmentDate.AddHours(2);
            fee1.Description = $"Fee su investimento in {vwce.Name}";

            var fee2 = new PlatformFee(
                feeNumber: $"FEE-{Guid.NewGuid().ToString()[..8]}",
                userId: investor.Id,
                type: PlatformFeeType.Investment,
                baseAmount: investment2.TotalAmount,
                feePercentage: 1.00m,
                minimumFee: 1.00m
            );
            fee2.RelatedInvestmentId = investment2.Id;
            fee2.TransactionDate = investment2.InvestmentDate;
            fee2.Status = FeeStatus.Collected;
            fee2.CollectedAt = investment2.InvestmentDate.AddHours(2);
            fee2.Description = $"Fee su investimento in {aapl.Name}";

            _context.PlatformFees.AddRange(fee1, fee2);

            portfolio.TotalInvested = investment1.TotalAmount + investment2.TotalAmount;
            portfolio.CurrentValue = portfolio.TotalInvested * 1.08m;

            var fundMovement = new FundMovement(
                movementNumber: $"FM-{Guid.NewGuid().ToString()[..8]}",
                userId: investor.Id,
                userAccountId: account.Id,
                companyAccountId: _companyAccountId,
                type: FundMovementType.Deposit,
                amount: investor.TotalDeposited
            );
            fundMovement.Status = FundMovementStatus.Completed;
            fundMovement.CompletedDate = DateTime.UtcNow.AddMonths(-3);
            _context.FundMovements.Add(fundMovement);
            await _context.SaveChangesAsync(); // Save fund movement

            // Platform Fee for fund movement
            var fmFee = new PlatformFee(
                feeNumber: $"FEE-{Guid.NewGuid().ToString()[..8]}",
                userId: investor.Id,
                type: PlatformFeeType.Deposit,
                baseAmount: fundMovement.Amount,
                feePercentage: 1.00m,
                minimumFee: 1.00m
            );
            fmFee.RelatedFundMovementId = fundMovement.Id;
            fmFee.TransactionDate = fundMovement.CompletedDate.Value;
            fmFee.Status = FeeStatus.Collected;
            fmFee.CollectedAt = fundMovement.CompletedDate.Value.AddMinutes(30);
            fmFee.Description = "Fee su movimento fondi";
            _context.PlatformFees.Add(fmFee);
        }

        private async Task PrintStatisticsAsync()
        {
            var usersCount = await _context.Users.CountAsync();
            var consultantsCount = await _context.ConsultantUsers.CountAsync();
            var investorsCount = await _context.InvestorUsers.CountAsync();
            var productsCount = await _context.InvestmentProducts.CountAsync();
            var badgesCount = await _context.Badges.CountAsync();
            var challengesCount = await _context.Challenges.CountAsync();
            var investmentsCount = await _context.Investments.CountAsync();
            var kycCount = await _context.KycVerifications.CountAsync();
            var kycVerifiedCount = await _context.KycVerifications.CountAsync(k => k.Status == KycStatus.Verified);
            var feesCount = await _context.PlatformFees.CountAsync();
            var totalFeesCollected = await _context.PlatformFees
                .Where(f => f.Status == FeeStatus.Collected)
                .SumAsync(f => f.CalculatedFee);

            Console.WriteLine($"\n?? Statistiche Database:");
            Console.WriteLine($"  ?? Utenti totali: {usersCount}");
            Console.WriteLine($"  ?? Consulenti: {consultantsCount}");
            Console.WriteLine($"  ?? Investitori: {investorsCount}");
            Console.WriteLine($"  ?? Prodotti finanziari: {productsCount}");
            Console.WriteLine($"  ?? Badge: {badgesCount}");
            Console.WriteLine($"  ?? Sfide: {challengesCount}");
            Console.WriteLine($"  ?? Investimenti attivi: {investmentsCount}");
            Console.WriteLine($"  ? KYC verificati: {kycVerifiedCount}/{kycCount}");
            Console.WriteLine($"  ?? Platform fees raccolte: €{totalFeesCollected:N2} ({feesCount} transazioni)");
        }
    }
}
