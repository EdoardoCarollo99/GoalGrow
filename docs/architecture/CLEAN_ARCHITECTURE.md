# Clean Architecture - GoalGrow

## Panoramica

GoalGrow implementa **Clean Architecture** (nota anche come Onion Architecture) per garantire:
- **Separation of Concerns**: Ogni layer ha responsabilità ben definite
- **Testabilità**: Business logic isolata da framework e infrastructure
- **Manutenibilità**: Modifiche localizzate senza effetti a cascata
- **Indipendenza da Framework**: Core business indipendente da EF Core, ASP.NET, etc.

---

## Struttura Layer

```
???????????????????????????????????????????????
?         Presentation Layer (API)            ?  ? Controllers, DTOs, Middlewares
???????????????????????????????????????????????
?       Application Layer (Services)          ?  ? Use Cases, Business Logic
???????????????????????????????????????????????
?         Domain Layer (Entity)               ?  ? Entities, Value Objects, Enums
???????????????????????????????????????????????
?     Infrastructure Layer (Data)             ?  ? EF Core, Repositories, DB Access
???????????????????????????????????????????????
```

### Dependency Flow
```
Presentation ? Application ? Domain ? Infrastructure
```

**Regola chiave**: Le dipendenze puntano **verso l'interno** (Domain è il core, non dipende da nessuno).

---

## Layer Dettagliati

### 1. Domain Layer (`GoalGrow.Entity`)

**Responsabilità**: Rappresenta il business core dell'applicazione.

**Contenuto**:
- `Models/` - Entità domain (User, Goal, Investment, Transaction)
- `Enums/` - Enumerazioni business (UserType, GoalStatus, RiskLevel)
- `ValueObjects/` - Oggetti valore (Money, DateRange, ContactInfo)
- `Super/` - Classi base astratte (User)

**Caratteristiche**:
- ? **ZERO dipendenze** esterne (solo .NET standard)
- ? Business rules incapsulati nelle entità
- ? Immutabilità per Value Objects
- ? Rich Domain Model (comportamenti + dati)

**Esempio Entity**:
```csharp
public class Goal
{
    // Domain logic
    public static Goal CreateEmergencyGoal(Guid userId, decimal targetAmount)
    {
        var goal = new Goal("Emergency Fund", targetAmount, 
            DateTime.UtcNow.AddMonths(12), userId);
        goal.Type = GoalType.Emergency;
        goal.IsSystemGoal = true;
        return goal;
    }

    // Business rules
    public bool CanUnlock()
    {
        return Type == GoalType.Emergency 
            ? CurrentAmount >= TargetAmount 
            : false;
    }
}
```

**Principi Applicati**:
- **Single Responsibility**: Ogni entità gestisce solo il proprio dominio
- **Encapsulation**: Stato modificabile solo tramite metodi pubblici
- **Invariants**: Regole di validazione sempre verificate

---

### 2. Infrastructure Layer (`GoalGrow.Data`)

**Responsabilità**: Implementazione tecnica per persistenza e accesso dati.

**Contenuto**:
- `GoalGrowDbContext.cs` - EF Core DbContext principale
- `Configurations/` - Fluent API configurations (FK, indici, constraints)
- `Contexts/` - DbContext modulari (Financial, Investment, Gamification)
- `Migrations/` - EF Core migration files

**Caratteristiche**:
- ? Dipende da **Domain Layer**
- ? EF Core 10.0
- ? Configuration separata per entity
- ? Performance ottimizzate (indici, query)

**Esempio Configuration**:
```csharp
public class GoalConfiguration : IEntityTypeConfiguration<Goal>
{
    public void Configure(EntityTypeBuilder<Goal> builder)
    {
        builder.HasKey(g => g.Id);
        
        // Indici per performance
        builder.HasIndex(g => new { g.UserId, g.Status });
        
        // Foreign key con Restrict
        builder.HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Precision per money
        builder.Property(g => g.TargetAmount).HasPrecision(18, 2);
    }
}
```

**Pattern Implementati**:
- **Repository Pattern** (via EF Core DbSet)
- **Unit of Work** (via DbContext)
- **Configuration Segregation** (un file per entity)

---

### 3. Application Layer (`GoalGrow.API/Services` - *planned*)

**Responsabilità**: Orchestrazione use cases e business logic applicativa.

**Contenuto** (da implementare):
- `Services/Interfaces/` - Contratti service
- `Services/Implementations/` - Implementazioni concrete
- `DTOs/` - Data Transfer Objects
- `Mappings/` - AutoMapper profiles
- `Validators/` - FluentValidation

**Caratteristiche**:
- ? Dipende da **Domain** e **Infrastructure**
- ? Implementa use cases
- ? Coordinamento tra repository
- ? Validazione input
- ? Transaction management

**Esempio Service**:
```csharp
public interface IGoalService
{
    Task<GoalResponse> CreateGoalAsync(CreateGoalRequest request, Guid userId);
    Task<GoalResponse> DepositToGoalAsync(Guid goalId, decimal amount, Guid userId);
    Task<List<GoalResponse>> GetUserGoalsAsync(Guid userId);
}

public class GoalService : IGoalService
{
    private readonly GoalGrowDbContext _context;
    private readonly ILogger<GoalService> _logger;

    public async Task<GoalResponse> CreateGoalAsync(CreateGoalRequest request, Guid userId)
    {
        // Validazione
        if (request.TargetAmount <= 0)
            throw new ValidationException("TargetAmount must be positive");

        // Business logic
        var goal = new Goal(request.Name, request.TargetAmount, 
            request.TargetDate, userId);
        
        _context.Goals.Add(goal);
        await _context.SaveChangesAsync();

        // Mapping to DTO
        return MapToResponse(goal);
    }
}
```

---

### 4. Presentation Layer (`GoalGrow.API`)

**Responsabilità**: Interfaccia HTTP REST per client esterni.

**Contenuto**:
- `Controllers/` - REST API endpoints
- `DTOs/Requests/` - Input DTOs
- `DTOs/Responses/` - Output DTOs
- `Middlewares/` - Error handling, logging
- `Extensions/` - Helper extensions

**Caratteristiche**:
- ? Dipende da **Application** e **Domain**
- ? ASP.NET Core 10.0
- ? JWT Authentication (Keycloak)
- ? Swagger/Scalar documentation
- ? Model validation

**Esempio Controller**:
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoalsController : ControllerBase
{
    private readonly IGoalService _goalService;

    [HttpGet]
    public async Task<IActionResult> GetMyGoals()
    {
        var userId = User.GetUserId(); // Extension method
        var goals = await _goalService.GetUserGoalsAsync(userId);
        return Ok(ApiResponse<List<GoalResponse>>.SuccessResponse(goals));
    }

    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
    {
        var userId = User.GetUserId();
        var goal = await _goalService.CreateGoalAsync(request, userId);
        return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, 
            ApiResponse<GoalResponse>.SuccessResponse(goal));
    }
}
```

---

## Dependency Injection

### Registrazione Services (Program.cs)

```csharp
// Infrastructure
builder.Services.AddDbContext<GoalGrowDbContext>(options =>
    options.UseSqlServer(connectionString));

// Application Services
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IInvestmentService, InvestmentService>();
builder.Services.AddScoped<IUserService, UserService>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => { /* Keycloak config */ });

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateGoalRequestValidator>();
```

---

## Vantaggi Architettura

### 1. Testabilità
```csharp
// Unit test business logic (Domain)
[Fact]
public void Goal_CanUnlock_WhenTargetReached()
{
    var goal = Goal.CreateEmergencyGoal(Guid.NewGuid(), 1000m);
    goal.CurrentAmount = 1000m;
    
    Assert.True(goal.CanUnlock());
}

// Integration test service (Application)
[Fact]
public async Task CreateGoal_ShouldPersist()
{
    var service = new GoalService(_mockContext, _mockLogger);
    var request = new CreateGoalRequest { Name = "Test", TargetAmount = 500 };
    
    var result = await service.CreateGoalAsync(request, _userId);
    
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}
```

### 2. Manutenibilità
- **Cambio DB**: Modifica solo Infrastructure Layer
- **Nuovo endpoint**: Aggiungi controller in Presentation
- **Nuova business rule**: Modifica solo Domain entity

### 3. Scalabilità
- **CQRS**: Separare Read/Write models
- **Event Sourcing**: Aggiungere event store
- **Microservices**: Estrarre bounded context

---

## Anti-Pattern da Evitare

### ? Domain dipende da Infrastructure
```csharp
// SBAGLIATO
public class Goal
{
    public void Save(GoalGrowDbContext context) // Domain dipende da EF!
    {
        context.Goals.Add(this);
        context.SaveChanges();
    }
}
```

### ? Controller contiene business logic
```csharp
// SBAGLIATO
[HttpPost]
public async Task<IActionResult> CreateGoal(CreateGoalRequest request)
{
    // Business logic nel controller!
    if (request.TargetAmount <= 0)
        return BadRequest();
    
    var goal = new Goal(request.Name, request.TargetAmount, ...);
    _context.Goals.Add(goal);
    await _context.SaveChangesAsync();
    
    return Ok(goal);
}
```

### ? Versione Corretta
```csharp
[HttpPost]
public async Task<IActionResult> CreateGoal(CreateGoalRequest request)
{
    // Delega a service layer
    var goal = await _goalService.CreateGoalAsync(request, User.GetUserId());
    return Ok(ApiResponse<GoalResponse>.SuccessResponse(goal));
}
```

---

## Roadmap Architettura

### Current State ?
- [x] Domain Layer completo
- [x] Infrastructure Layer con EF Core
- [x] Presentation Layer base (auth endpoints)

### Next Steps ??
- [ ] Application Services
- [ ] AutoMapper configurations
- [ ] FluentValidation
- [ ] Unit Tests (Domain)
- [ ] Integration Tests (Services)

### Future Enhancements ??
- [ ] CQRS pattern
- [ ] MediatR per use cases
- [ ] Event Sourcing
- [ ] Domain Events
- [ ] Specification Pattern

---

## Risorse

- [Clean Architecture (Robert C. Martin)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Clean Architecture Template](https://github.com/jasontaylordev/CleanArchitecture)
- [DDD in Practice](https://app.pluralsight.com/library/courses/domain-driven-design-in-practice)

---

**Autore**: Edoardo Carollo  
**Data**: 2025-01-18  
**Versione**: 1.0.0
