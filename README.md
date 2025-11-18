#  GoalGrow

> Smart Financial Management Platform with Savings Goals, Investment Marketplace & Gamification

[![.NET](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Status](https://img.shields.io/badge/Status-In%20Development-yellow)](CHANGELOG.md)

---

##  What is GoalGrow

**GoalGrow** is a fintech platform that helps users save money through guided goals and connect with financial consultants when they're ready to invest.

### Key Features

-  **Smart Savings** - Auto-created Emergency & Investment funds
-  **Goal Tracking** - Custom savings goals (vacation, car, education)
-  **Investment Marketplace** - Connect with verified financial consultants
-  **Gamification** - XP, levels, badges, and challenges with rewards
-  **KYC Compliant** - Secure identity verification

---

##  Technology Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | .NET 10, ASP.NET Core Web API |
| **Frontend** | Blazor Server + MAUI (Mobile) |
| **Database** | SQL Server + Entity Framework Core 10 |
| **Authentication** | Keycloak (OpenID Connect / OAuth 2.0) |
| **Storage** | Azure Blob Storage (KYC documents) |
| **Architecture** | Clean Architecture, Domain-Driven Design |

---

##  Quick Start

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for Keycloak)

### 1. Clone Repository

```bash
git clone https://github.com/EdoardoCarollo99/GoalGrow.git
cd GoalGrow
```

### 2. Setup Database

```bash
# Configure connection string
cd GoalGrow.Migration
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# Run migrations and seed data
dotnet run
```

This will:
-  Create database schema
-  Seed sample users, goals, and challenges
-  Generate test data for development

### 3. Start Keycloak (Authentication)

```bash
# In project root
docker-compose up -d
```

Access Keycloak at: http://localhost:8080
- Username: `admin`
- Password: `admin`

### 4. Run Application

```bash
# API (future)
cd GoalGrow.Api
dotnet run

# Web App (future)
cd GoalGrow.Web
dotnet run
```

---

##  Project Status

**Current Phase:** Foundation & Database Setup  
**MVP Target:** Q4 2025 (11 months)

See [CHANGELOG.md](CHANGELOG.md) for completed work and [docs/ROADMAP.md](docs/ROADMAP.md) for upcoming features.

---

##  Documentation

Comprehensive documentation is available in the [`/docs`](docs/) folder:

- **[System Overview](docs/SYSTEM_OVERVIEW.md)** - Architecture and modules
- **[Getting Started](docs/GETTING_STARTED.md)** - Detailed setup guide
- **[Authentication](docs/AUTHENTICATION.md)** - Keycloak integration
- **[Database](docs/DATABASE.md)** - Schema and migrations
- **[API Reference](docs/API_REFERENCE.md)** - Endpoints (future)
- **[Roadmap](docs/ROADMAP.md)** - Development timeline

 **Diagrams:** [docs/diagrams/](docs/diagrams/)

---

##  Business Model

GoalGrow generates revenue through:
- **Platform Fees:** 1% (min €1) on deposits, withdrawals, investments
- **Consultant Marketplace:** 20% of consultant commissions

**Target Market:** Retail users (25-45 years) in Italy/EU who want guided savings and investment advice.

---

##  User Journey

```
Registration  KYC Verification  Save Money  Reach Threshold  
Find Consultant  Invest  Track Performance  Earn Rewards
```

See [docs/USER_JOURNEY.md](docs/USER_JOURNEY.md) for detailed flows.

---

##  Development

### Project Structure

```
GoalGrow/
 GoalGrow.Entity/          # Domain models & value objects
 GoalGrow.Data/            # EF Core DbContext & configurations
 GoalGrow.Migration/       # Database migrations & seeding
 GoalGrow.Api/             # Web API (future)
 GoalGrow.Web/             # Blazor Web App (future)
 docs/                     # Documentation
 Diagrams/                 # PlantUML diagrams
```

### Running Migrations

```bash
cd GoalGrow.Migration

# Create new migration
dotnet ef migrations add MigrationName --project ../GoalGrow.Data --startup-project .

# Apply migrations
dotnet ef database update --project ../GoalGrow.Data --startup-project .

# Rollback
dotnet ef database update PreviousMigrationName --project ../GoalGrow.Data --startup-project .
```

### Database Seeding

The seeder creates:
- 3 test users (Investor, Consultant, Admin)
- System goals (Emergency & Investment funds)
- Sample transactions and portfolios
- Gamification data (badges, challenges)

Run: `dotnet run` in `GoalGrow.Migration` project.

---

##  Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

##  License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file.

---

##  Contact

**Edoardo Carollo**  
- GitHub: [@EdoardoCarollo99](https://github.com/EdoardoCarollo99)
- Repository: [GoalGrow](https://github.com/EdoardoCarollo99/GoalGrow)

---

##  Acknowledgments

Built with modern .NET practices:
- Domain-Driven Design (DDD)
- Clean Architecture
- CQRS principles (future)
- Event Sourcing (future)

---

** Ready to start** See [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md)
