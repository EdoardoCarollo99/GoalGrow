#  GoalGrow - Technical Roadmap

## Current Status 

###  Completed (Today)
- [x] Domain models refactored (Value Objects, Base Classes)
- [x] Modular architecture documentation
- [x] PlantUML diagrams for all modules
- [x] KYC entity created
- [x] PlatformFee entity created
- [x] Goal enhanced with system goals (Emergency, Investment)
- [x] Authentication architecture documented
- [x] Business requirements documented

---

##  Next Steps (Immediate Priority)

### Step 1: Apply Refactoring Migration (1 day)

**Run the migration script:**
```powershell
cd C:\Users\edoardo.carollo\source\repos\Personale\GoalGrowe
.\Migrate-Configurations.ps1 -Backup
```

**Checklist:**
- [ ] Backup created
- [ ] Configuration files moved to module folders
- [ ] Namespaces updated
- [ ] Build successful: `dotnet build`
- [ ] No migration errors

---

### Step 2: Update Entity Models (2-3 days)

#### 2.1 Add New Fields to Existing Entities

**User.cs:**
```csharp
// Add Keycloak integration
[Required]
[MaxLength(255)]
public string KeycloakSubjectId { get; set; } = string.Empty;
```

**InversotorUser.cs:**
```csharp
// Add KYC relationship
public virtual KycVerification KycVerification { get; set; }
```

#### 2.2 Create Missing Enum Files
- [ ] `GoalType.cs`  (Already created)
- [ ] `KycStatus.cs`  (Already created)
- [ ] `PlatformFeeType.cs`  (Already created)

#### 2.3 Update DbContext
```csharp
// Add new DbSets
public DbSet<KycVerification> KycVerifications { get; set; }
public DbSet<PlatformFee> PlatformFees { get; set; }
```

---

### Step 3: Create EF Configurations (1 day)

**Files to create:**
```
GoalGrow.Data/Configurations/
 Compliance/
    KycVerificationConfiguration.cs
 Financial/
     PlatformFeeConfiguration.cs
```

**Example: KycVerificationConfiguration.cs**
```csharp
public class KycVerificationConfiguration : IEntityTypeConfiguration<KycVerification>
{
    public void Configure(EntityTypeBuilder<KycVerification> builder)
    {
        builder.HasKey(k => k.Id);
        
        builder.HasIndex(k => k.UserId).IsUnique();
        builder.HasIndex(k => k.Status);
        builder.HasIndex(k => new { k.Status, k.SubmittedAt });
        
        builder.HasOne(k => k.User)
            .WithOne(u => u.KycVerification)
            .HasForeignKey<KycVerification>(k => k.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

---

### Step 4: Create Migration (30 min)

```bash
cd GoalGrow.Migration

# Create new migration
dotnet ef migrations add AddKycAndPlatformFees --project ../GoalGrow.Data --startup-project .

# Review migration file (check for issues)
# Apply migration
dotnet ef database update --project ../GoalGrow.Data --startup-project .
```

**Verify:**
- [ ] New tables created: `KycVerifications`, `PlatformFees`
- [ ] New column in `Users`: `KeycloakSubjectId`
- [ ] New columns in `Goals`: `Type`, `IsSystemGoal`, `UnlockThreshold`, etc.
- [ ] No errors in migration

---

### Step 5: Set Up Keycloak (2-3 hours)

#### Option A: Docker (Recommended for Dev)

```bash
# Create docker-compose.yml
docker-compose up -d

# Access Keycloak
# http://localhost:8080
# Username: admin
# Password: admin
```

**Configuration:**
1. Create Realm: `GoalGrow-Dev`
2. Create Clients:
   - `goalgrow-api` (Backend API)
   - `goalgrow-web` (Blazor Web)
3. Create Roles:
   - `investor`
   - `consultant`
   - `admin`
   - `kyc-verified`
4. Create test users

#### Option B: Cloud (Production)

Use managed Keycloak (e.g., Red Hat SSO on Azure)

---

### Step 6: Create API Project (1 week)

```bash
cd C:\Users\edoardo.carollo\source\repos\Personale\GoalGrowe

# Create new Web API project
dotnet new webapi -n GoalGrow.Api -o GoalGrow.Api --framework net10.0

# Add project to solution
dotnet sln add GoalGrow.Api/GoalGrow.Api.csproj

# Add references
cd GoalGrow.Api
dotnet add reference ../GoalGrow.Data/GoalGrow.Data.csproj
dotnet add reference ../GoalGrow.Entity/GoalGrow.Entity.csproj

# Add NuGet packages
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Swashbuckle.AspNetCore
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection
```

**Folder Structure:**
```
GoalGrow.Api/
 Controllers/
    AuthController.cs
    UserController.cs
    GoalController.cs
    InvestmentController.cs
    ConsultantController.cs
    KycController.cs
 DTOs/
    Requests/
    Responses/
 Services/
    Interfaces/
    Implementations/
 Middleware/
    ExceptionHandlingMiddleware.cs
    AuthenticationMiddleware.cs
 Mappings/
    AutoMapperProfile.cs
 Program.cs
```

---

### Step 7: Implement Authentication in API (2-3 days)

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

// Add Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        options.Audience = "goalgrow-api";
        options.RequireHttpsMetadata = true;
    });

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InvestorOnly", policy => policy.RequireRole("investor"));
    options.AddPolicy("ConsultantOnly", policy => policy.RequireRole("consultant"));
    options.AddPolicy("KycVerified", policy => policy.RequireRole("kyc-verified"));
});

// Add DbContext
builder.Services.AddDbContext<GoalGrowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GoalGrowDb")));

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IKycService, KycService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

**Create Services:**
- `IUserService` - User management, sync with Keycloak
- `IGoalService` - Goal CRUD, auto-create system goals
- `IKycService` - KYC verification workflow
- `IInvestmentService` - Investment operations
- `IPlatformFeeService` - Calculate and apply fees

---

### Step 8: Create Blazor Web App (1 week)

```bash
# Create Blazor Server project
dotnet new blazorserver -n GoalGrow.Web -o GoalGrow.Web --framework net10.0

# Add to solution
dotnet sln add GoalGrow.Web/GoalGrow.Web.csproj

# Add packages
cd GoalGrow.Web
dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
dotnet add package Radzen.Blazor  # UI component library
```

**Pages to Create:**
```
Pages/
 Auth/
    Login.razor
    Register.razor
    Logout.razor
 Dashboard/
    Index.razor
    Goals.razor
    Transactions.razor
    Portfolio.razor
 Kyc/
    Upload.razor
    Status.razor
 Investment/
    Marketplace.razor
    ConsultantProfile.razor
    RiskProfile.razor
 Settings/
     Profile.razor
     Security.razor
```

---

### Step 9: Implement Core Features (3-4 weeks)

#### Week 1: User & Goal Management
- [ ] User registration flow
- [ ] Auto-create Emergency + Investment goals
- [ ] Goal CRUD operations
- [ ] Deposit/Withdrawal with fees
- [ ] Virtual wallet balance tracking

#### Week 2: KYC System
- [ ] Document upload (Azure Blob Storage)
- [ ] KYC status workflow
- [ ] Admin verification dashboard
- [ ] Integration with Onfido/Jumio (future)

#### Week 3: Investment System
- [ ] RiskProfile questionnaire
- [ ] Consultant marketplace (list + search)
- [ ] Portfolio creation
- [ ] Investment product catalog

#### Week 4: Gamification
- [ ] XP system
- [ ] Badge unlocking
- [ ] Challenge mechanics
- [ ] Leaderboard

---

### Step 10: Testing (2 weeks)

#### Unit Tests
```bash
# Create test project
dotnet new xunit -n GoalGrow.Tests -o GoalGrow.Tests
dotnet sln add GoalGrow.Tests/GoalGrow.Tests.csproj

# Add test packages
cd GoalGrow.Tests
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

**Test Coverage:**
- [ ] UserService tests
- [ ] GoalService tests (system goal creation)
- [ ] PlatformFeeService tests (fee calculation)
- [ ] KycService tests
- [ ] Authentication tests

#### Integration Tests
- [ ] API endpoint tests
- [ ] Database integration tests
- [ ] Authentication flow tests

---

### Step 11: Deployment (1 week)

#### Azure Resources
```bash
# Create resource group
az group create --name rg-goalgrow-prod --location westeurope

# Create SQL Database
az sql server create --name sql-goalgrow-prod --resource-group rg-goalgrow-prod --location westeurope
az sql db create --name goalgrow-db --server sql-goalgrow-prod --resource-group rg-goalgrow-prod

# Create App Service
az appservice plan create --name plan-goalgrow-prod --resource-group rg-goalgrow-prod --sku B1
az webapp create --name app-goalgrow-api --resource-group rg-goalgrow-prod --plan plan-goalgrow-prod

# Create Storage Account (for KYC docs)
az storage account create --name stgoalgrowprod --resource-group rg-goalgrow-prod --location westeurope
```

#### CI/CD with GitHub Actions
```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [ main ]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: '10.0.x'
      - name: Build
        run: dotnet build --configuration Release
      - name: Test
        run: dotnet test
      - name: Publish
        run: dotnet publish -c Release -o ./publish
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'app-goalgrow-api'
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
```

---

##  Timeline Summary

| Phase | Duration | Tasks |
|-------|----------|-------|
| **Phase 1: Foundation** | 1 week | Migration, EF updates, Keycloak setup |
| **Phase 2: API Development** | 3 weeks | Create API project, auth, core services |
| **Phase 3: Web App** | 2 weeks | Blazor app, basic pages |
| **Phase 4: Features** | 4 weeks | KYC, Investment, Gamification |
| **Phase 5: Testing** | 2 weeks | Unit + Integration tests |
| **Phase 6: Deployment** | 1 week | Azure setup, CI/CD |

**Total MVP: ~13 weeks (3 months)**

---

##  Success Criteria

### Technical
- [ ] 100% API test coverage
- [ ] <200ms API response time (p95)
- [ ] Zero downtime deployments
- [ ] GDPR compliant data handling
- [ ] OWASP Top 10 security compliance

### Business
- [ ] 95% KYC completion rate
- [ ] 80% system goal activation (Emergency + Investment)
- [ ] 50% investment threshold unlock rate
- [ ] <2% user churn rate (monthly)

---

##  Resources

### Documentation
- [AUTHENTICATION_ARCHITECTURE.md](AUTHENTICATION_ARCHITECTURE.md)
- [BUSINESS_REQUIREMENTS.md](BUSINESS_REQUIREMENTS.md)
- [ARCHITECTURE_V2.md](ARCHITECTURE_V2.md)

### External
- [Keycloak Docs](https://www.keycloak.org/documentation)
- [Blazor Docs](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
- [Azure App Service](https://learn.microsoft.com/en-us/azure/app-service/)

---

##  Let's Start!

**Next immediate action:**
```powershell
# Run migration script
.\Migrate-Configurations.ps1 -Backup
```

Then create a new branch for Keycloak integration:
```bash
git checkout -b feature/keycloak-auth
```

**Ready to proceed** 
