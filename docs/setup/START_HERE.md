#  Quick Start Checklist

Use this checklist to get GoalGrow running in under 10 minutes!

---

##  Step-by-Step Setup

### 1. Prerequisites Installed 
- [ ] .NET 10 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/10.0))
- [ ] SQL Server or SQL Server Express ([Download](https://www.microsoft.com/sql-server/sql-server-downloads))
- [ ] Docker Desktop ([Download](https://www.docker.com/products/docker-desktop))
- [ ] Visual Studio 2025 or VS Code

### 2. Clone Repository 
```bash
git clone https://github.com/EdoardoCarollo99/GoalGrow.git
cd GoalGrow
```

### 3. Run Automated Setup 
```powershell
.\Setup-Development.ps1
```

**What this does:**
-  Configures User Secrets
-  Creates database
-  Runs migrations
-  Seeds sample data
-  Starts Keycloak (Docker)

**Alternative (Manual Setup):**
```bash
cd GoalGrow.Migration
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run
```

### 4. Verify Setup 
- [ ] Database `GoalGrowDb` created
- [ ] Sample data visible in SQL Server
- [ ] Keycloak accessible at http://localhost:8080

**Test Query:**
```sql
SELECT * FROM Users;
SELECT * FROM Goals;
SELECT * FROM InvestmentProducts;
```

---

##  What's Next

### Explore the Code
- [ ] Open solution in Visual Studio
- [ ] Browse `GoalGrow.Entity/Models/` to see domain entities
- [ ] Check `DatabaseSeeder.cs` for sample data

### Read Documentation
- [ ] [docs/INDEX.md](docs/INDEX.md) - Documentation hub
- [ ] [docs/GETTING_STARTED.md](docs/GETTING_STARTED.md) - Detailed setup guide
- [ ] [docs/technical/ARCHITECTURE.md](docs/technical/ARCHITECTURE.md) - System architecture

### View Diagrams
- [ ] Open `.puml` files in `docs/diagrams/` with PlantUML extension
- [ ] Or paste into [PlantUML Online](http://www.plantuml.com/plantuml/uml/)

### Configure Keycloak
- [ ] Access http://localhost:8080 (admin/admin)
- [ ] Create realm: `GoalGrow-Dev`
- [ ] Configure clients (see [docs/technical/AUTHENTICATION.md](docs/technical/AUTHENTICATION.md))

### Start Developing
- [ ] Create API project (see [docs/ROADMAP.md](docs/ROADMAP.md))
- [ ] Implement authentication
- [ ] Build first endpoints

---

##  Troubleshooting

### Database Connection Issues
```bash
# Check SQL Server is running
# Verify connection string in User Secrets
dotnet user-secrets list --project GoalGrow.Migration
```

### Keycloak Not Starting
```bash
# Check Docker is running
docker ps

# View logs
docker-compose logs keycloak
```

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

---

##  Sample Data Overview

After running setup, you'll have:

**3 Users:**
- Admin (admin@goalgrow.com)
- Investor (investor@goalgrow.com)  
- Consultant (consultant@goalgrow.com)

**System Goals:**
- Emergency Fund (€3,000 target)
- Investment Fund (€5,000 target)

**Investment Products:**
- 5 stocks (AAPL, MSFT, GOOGL, etc.)
- 2 cryptocurrencies (BTC, ETH)
- 3 ETFs

**Gamification:**
- 10 badges
- 5 challenges

---

##  Setup Complete Checklist

- [ ] Database created and seeded 
- [ ] Keycloak running 
- [ ] Sample data verified 
- [ ] Documentation reviewed 
- [ ] Ready to start coding! 

---

** You're all set! Happy coding!**

**Next:** See [docs/ROADMAP.md](docs/ROADMAP.md) for development plan.
