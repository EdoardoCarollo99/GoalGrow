#  Getting Started with GoalGrow

This guide will help you set up GoalGrow for development in under 10 minutes.

---

##  Prerequisites

Before you begin, ensure you have:

-  **[.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)** installed
-  **[SQL Server](https://www.microsoft.com/sql-server)** or **SQL Server Express**
-  **[Docker Desktop](https://www.docker.com/products/docker-desktop)** (for Keycloak)
-  **[Visual Studio 2025](https://visualstudio.microsoft.com/)** or **[VS Code](https://code.visualstudio.com/)**
-  **[Git](https://git-scm.com/)** for version control

### Optional Tools
- **[SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/)** - Database management
- **[Postman](https://www.postman.com/)** - API testing (future)
- **[PlantUML Extension](https://marketplace.visualstudio.com/itemsitemName=jebbs.plantuml)** - View diagrams

---

##  Quick Setup (5 minutes)

### Step 1: Clone Repository

```bash
git clone https://github.com/EdoardoCarollo99/GoalGrow.git
cd GoalGrow
```

### Step 2: Configure Database Connection

```bash
cd GoalGrow.Migration

# Option 1: Using SQL Server with Windows Authentication
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# Option 2: Using SQL Server with SQL Authentication
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=localhost;Database=GoalGrowDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True"

# Option 3: Using LocalDB (Visual Studio)
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=(localdb)\\mssqllocaldb;Database=GoalGrowDb;Trusted_Connection=True"
```

### Step 3: Create Database & Seed Data

```bash
# Still in GoalGrow.Migration folder
dotnet run
```

This command will:
1.  Create database schema (all tables)
2.  Apply migrations
3.  Seed sample data:
   - 3 users (Admin, Investor, Consultant)
   - System goals (Emergency, Investment)
   - Sample transactions
   - Investment products
   - Badges and challenges

**Expected output:**
```
 Starting database seeding...
 Users seeded
 Goals seeded
 Transactions seeded
 Investment products seeded
 Badges seeded
 Challenges seeded
 Database seeding completed successfully!
```

### Step 4: Verify Setup

```bash
# Check if database exists
dotnet ef database list --project ../GoalGrow.Data --startup-project .

# View migration history
dotnet ef migrations list --project ../GoalGrow.Data --startup-project .
```

---

##  Keycloak Setup (Authentication)

### Option 1: Docker (Recommended for Development)

Create `docker-compose.yml` in project root:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: keycloak
      POSTGRES_USER: keycloak
      POSTGRES_PASSWORD: password
    volumes:
      - keycloak_db:/var/lib/postgresql/data
    networks:
      - keycloak-network

  keycloak:
    image: quay.io/keycloak/keycloak:latest
    environment:
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: password
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
    ports:
      - "8080:8080"
    command: start-dev
    depends_on:
      - postgres
    networks:
      - keycloak-network

volumes:
  keycloak_db:

networks:
  keycloak-network:
```

**Start Keycloak:**

```bash
docker-compose up -d
```

**Access Keycloak Admin Console:**
- URL: http://localhost:8080
- Username: `admin`
- Password: `admin`

### Option 2: Manual Installation

Download from [Keycloak.org](https://www.keycloak.org/downloads) and follow installation instructions.

### Configure Keycloak

1. **Create Realm:**
   - Name: `GoalGrow-Dev`

2. **Create Clients:**
   - `goalgrow-api` (Backend)
     - Client Protocol: `openid-connect`
     - Access Type: `confidential`
     - Valid Redirect URIs: `https://localhost:5001/*`
   - `goalgrow-web` (Frontend)
     - Client Protocol: `openid-connect`
     - Access Type: `public`
     - Valid Redirect URIs: `https://localhost:7001/*`

3. **Create Roles:**
   - `investor`
   - `consultant`
   - `admin`
   - `kyc-verified`

4. **Create Test Users:**
   - `investor@test.com` (role: investor)
   - `consultant@test.com` (role: consultant)
   - `admin@test.com` (role: admin)

See [technical/AUTHENTICATION.md](technical/AUTHENTICATION.md) for detailed Keycloak configuration.

---

##  Database Management

### Connection Strings

**Development:**
```
Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True
```

**LocalDB:**
```
Server=(localdb)\\mssqllocaldb;Database=GoalGrowDb;Trusted_Connection=True
```

**Azure SQL:**
```
Server=tcp:your-server.database.windows.net,1433;Database=GoalGrowDb;User ID=admin;Password=YourPassword;Encrypt=True;
```

### Common Database Commands

```bash
# List all migrations
dotnet ef migrations list --project GoalGrow.Data --startup-project GoalGrow.Migration

# Create new migration
dotnet ef migrations add MigrationName --project GoalGrow.Data --startup-project GoalGrow.Migration

# Apply migrations
dotnet ef database update --project GoalGrow.Data --startup-project GoalGrow.Migration

# Rollback to specific migration
dotnet ef database update PreviousMigration --project GoalGrow.Data --startup-project GoalGrow.Migration

# Drop database ( destructive!)
dotnet ef database drop --project GoalGrow.Data --startup-project GoalGrow.Migration --force

# Re-seed data
cd GoalGrow.Migration
dotnet run
```

### View Sample Data

Connect to database with SSMS or Azure Data Studio:

**Sample Users:**
| Email | Password | Role | KeycloakSubjectId |
|-------|----------|------|-------------------|
| admin@goalgrow.com | - | Admin | keycloak-admin-123 |
| investor@goalgrow.com | - | Investor | keycloak-investor-123 |
| consultant@goalgrow.com | - | Consultant | keycloak-consultant-123 |

**Sample Goals:**
- Emergency Fund (€3,000 target)
- Investment Fund (€5,000 target, unlocks marketplace)
- Vacation 2025 (€2,000 target)

**Investment Products:**
- Stocks: Apple (AAPL), Microsoft (MSFT)
- Crypto: Bitcoin (BTC-EUR), Ethereum (ETH-EUR)
- ETFs: Vanguard S&P 500 (VOO)

---

##  Project Structure

```
GoalGrow/
 GoalGrow.Entity/              # Domain Layer
    Common/                   # Base classes
    Models/                   # Entities
    Enums/                    # Enumerations
    ValueObjects/             # Value Objects (Money, DateRange)
    Super/                    # Abstract User class

 GoalGrow.Data/                # Infrastructure Layer
    Configurations/           # EF Core configurations
       User/
       Financial/
       Investment/
       Gamification/
    Contexts/                 # Module-specific DbContexts
    GoalGrowDbContext.cs      # Main DbContext

 GoalGrow.Migration/           # Database Management
    Program.cs                # Seeder runner
    DatabaseSeeder.cs         # Seed data logic
    GoalGrowDbContextFactory.cs

 docs/                         # Documentation
    INDEX.md                  # Documentation hub
    GETTING_STARTED.md        # This file
    diagrams/                 # PlantUML diagrams
    technical/                # Technical docs

 CHANGELOG.md                  # Version history
```

---

##  Testing Your Setup

### 1. Build Solution

```bash
dotnet build
```

### 2. Run Tests (future)

```bash
dotnet test
```

### 3. Verify Database

Query sample data:

```sql
-- Check users
SELECT * FROM Users;

-- Check goals
SELECT * FROM Goals;

-- Check investment products
SELECT * FROM InvestmentProducts;

-- Check badges
SELECT * FROM Badges;
```

---

##  Next Steps

Now that your environment is set up:

1. **Explore the Code**
   - Start with `GoalGrow.Entity/Models/` to understand domain entities
   - Review `DatabaseSeeder.cs` to see sample data structure

2. **Read Documentation**
   - [System Overview](SYSTEM_OVERVIEW.md) - Architecture
   - [Database Schema](technical/DATABASE.md) - Table relationships
   - [Domain Models](technical/DOMAIN_MODELS.md) - Entity reference

3. **View Diagrams**
   - Open `.puml` files in `docs/diagrams/` with PlantUML extension
   - Or paste content into [PlantUML Online](http://www.plantuml.com/plantuml/uml/)

4. **Start Developing**
   - Create API project (see [ROADMAP.md](ROADMAP.md))
   - Build Blazor Web App
   - Implement authentication with Keycloak

---

##  Troubleshooting

### Database Connection Issues

**Error:** "Cannot open database 'GoalGrowDb'"

**Solution:**
1. Verify SQL Server is running
2. Check connection string in User Secrets
3. Ensure database was created: `dotnet ef database update`

---

### Migration Issues

**Error:** "No migrations configuration type was found"

**Solution:**
- Ensure you're running commands from `GoalGrow.Migration` folder
- Use `--project` and `--startup-project` flags

---

### Keycloak Issues

**Error:** "Connection refused localhost:8080"

**Solution:**
- Ensure Docker Desktop is running
- Run `docker-compose up -d`
- Check logs: `docker-compose logs keycloak`

---

##  Additional Resources

- [.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

##  Setup Checklist

- [ ] .NET 10 SDK installed
- [ ] SQL Server installed and running
- [ ] Repository cloned
- [ ] User Secrets configured
- [ ] Database created and seeded
- [ ] Docker Desktop installed
- [ ] Keycloak running (docker-compose up)
- [ ] Keycloak realm configured
- [ ] Sample data verified in database
- [ ] Solution builds successfully

---

** You're ready to start developing!**

See [ROADMAP.md](ROADMAP.md) for the development plan or [technical/API_REFERENCE.md](technical/API_REFERENCE.md) to start building the API.
