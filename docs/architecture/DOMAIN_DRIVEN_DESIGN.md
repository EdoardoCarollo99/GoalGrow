# Domain-Driven Design - GoalGrow

## Introduzione

GoalGrow applica i principi di **Domain-Driven Design (DDD)** per modellare il dominio finanziario complesso in modo chiaro e manutenibile.

---

## Bounded Contexts

Il sistema GoalGrow è suddiviso in **Bounded Context** separati:

```
???????????????????????????????????????????????????
?              GoalGrow System                     ?
?                                                  ?
?  ????????????????  ??????????????????????????   ?
?  ?  User        ?  ?  Financial Management  ?   ?
?  ?  Management  ?  ?  - Goals               ?   ?
?  ?  - Admin     ?  ?  - Budgets             ?   ?
?  ?  - Consultant?  ?  - Transactions        ?   ?
?  ?  - Investor  ?  ?  - Accounts            ?   ?
?  ????????????????  ??????????????????????????   ?
?                                                  ?
?  ????????????????  ??????????????????????????   ?
?  ?  Investment  ?  ?  Gamification          ?   ?
?  ?  System      ?  ?  - Badges              ?   ?
?  ?  - Portfolio ?  ?  - Challenges          ?   ?
?  ?  - Products  ?  ?  - Levels              ?   ?
?  ?  - Trades    ?  ?  - Rewards             ?   ?
?  ????????????????  ??????????????????????????   ?
?                                                  ?
?  ????????????????  ??????????????????????????   ?
?  ?  Compliance  ?  ?  Marketplace           ?   ?
?  ?  - KYC/AML   ?  ?  - Consultants         ?   ?
?  ?  - Risk      ?  ?  - Matching            ?   ?
?  ?  - Audit     ?  ?  - Commissions         ?   ?
?  ????????????????  ??????????????????????????   ?
???????????????????????????????????????????????????
```

---

## Aggregates

### 1. User Aggregate

**Aggregate Root**: `User` (base astratta)

**Entità Figlie**:
- `AdminUser` (extends User)
- `ConsultantUser` (extends User)
- `InversotorUser` (extends User)

**Value Objects**:
- `ContactInfo` (email, phone)
- `Address` *(planned)*

**Invariants**:
- Email deve essere unique
- Phone number deve essere valido
- UserType non modificabile dopo creazione

```csharp
// Aggregate Root
public abstract class User
{
    public Guid Id { get; private set; }
    public string EmailAddress { get; private set; }
    public UserType UserType { get; private set; }
    
    // Factory Method
    public static InversotorUser CreateInvestor(
        string firstName, string lastName, 
        string email, string phone,
        string fiscalCode, DateTime birthDate)
    {
        // Validazioni invariant
        if (string.IsNullOrEmpty(email))
            throw new DomainException("Email required");
        
        return new InversotorUser(firstName, lastName, phone, 
            email, fiscalCode, birthDate);
    }
}
```

---

### 2. Goal Aggregate

**Aggregate Root**: `Goal`

**Entità Figlie**: Nessuna (aggregate semplice)

**Value Objects**:
- `Money` (amount, currency) *(planned)*
- `DateRange` (start, end) *(planned)*

**Relazioni**:
- Belongs to `User` (FK)
- Has many `Transactions` (associazione)

**Invariants**:
- TargetAmount > 0
- CurrentAmount >= 0
- CurrentAmount <= TargetAmount
- TargetDate > CreatedAt

**Domain Events** *(planned)*:
- `GoalCreated`
- `GoalCompleted`
- `GoalUnlocked`
- `DepositMadeToGoal`

```csharp
public class Goal
{
    // Factory methods per goal di sistema
    public static Goal CreateEmergencyGoal(Guid userId, decimal targetAmount)
    {
        var goal = new Goal("Emergency Fund", targetAmount, 
            DateTime.UtcNow.AddMonths(12), userId);
        
        goal.Type = GoalType.Emergency;
        goal.IsSystemGoal = true;
        goal.Priority = GoalPriority.High;
        goal.Description = "Fondo emergenze obbligatorio (sempre svincolabile)";
        
        return goal;
    }
    
    public static Goal CreateInvestmentGoal(Guid userId, decimal unlockThreshold)
    {
        var goal = new Goal("Investment Fund", unlockThreshold, 
            DateTime.UtcNow.AddYears(1), userId);
        
        goal.Type = GoalType.Investment;
        goal.IsSystemGoal = true;
        goal.UnlockThreshold = unlockThreshold; // €5,000 default
        goal.Description = "Sblocca marketplace consulenti raggiungendo soglia";
        
        return goal;
    }
    
    // Business logic
    public void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new DomainException("Amount must be positive");
        
        if (Status == GoalStatus.Completed)
            throw new DomainException("Cannot deposit to completed goal");
        
        CurrentAmount += amount;
        
        // Check completion
        if (CurrentAmount >= TargetAmount)
        {
            Status = GoalStatus.Completed;
            CompletedDate = DateTime.UtcNow;
            // Raise domain event: GoalCompleted
        }
    }
    
    public bool CanUnlock()
    {
        return Type switch
        {
            GoalType.Emergency => CurrentAmount >= TargetAmount,
            GoalType.Investment => CurrentAmount >= UnlockThreshold,
            _ => false
        };
    }
}
```

---

### 3. Investment Aggregate

**Aggregate Root**: `Portfolio`

**Entità Figlie**:
- `Investment` (composizione)
- `PortfolioSnapshot` (history)

**Value Objects**:
- `Money` (amount, currency)
- `Quantity` (decimal, unit)

**Invariants**:
- Portfolio.TotalInvested = Sum(Investments.TotalAmount)
- Investment.Quantity > 0
- Investment.PurchasePrice > 0

**Domain Events**:
- `InvestmentCreated`
- `InvestmentSold`
- `PortfolioRebalanced`

```csharp
public class Portfolio
{
    private readonly List<Investment> _investments = new();
    public IReadOnlyCollection<Investment> Investments => _investments.AsReadOnly();
    
    public void AddInvestment(InvestmentProduct product, decimal quantity, decimal price)
    {
        // Validazioni business
        if (quantity <= 0)
            throw new DomainException("Quantity must be positive");
        
        if (price < product.MinimumInvestment)
            throw new DomainException($"Minimum investment: {product.MinimumInvestment}");
        
        var investment = new Investment(
            UserId, Id, product.Id,
            GenerateInvestmentNumber(),
            quantity, price, quantity * price
        );
        
        _investments.Add(investment);
        RecalculateTotals();
        
        // Raise event: InvestmentCreated
    }
    
    private void RecalculateTotals()
    {
        TotalInvested = _investments
            .Where(i => i.Status == InvestmentStatus.Active)
            .Sum(i => i.TotalAmount);
        
        CurrentValue = _investments
            .Where(i => i.Status == InvestmentStatus.Active)
            .Sum(i => i.CurrentValue);
    }
}
```

---

## Entities vs Value Objects

### Entities
**Caratteristiche**:
- Hanno **identità** (Id univoco)
- **Mutabili** nel tempo
- **Uguaglianza** basata su Id

**Esempi in GoalGrow**:
- `User`, `Goal`, `Investment`, `Transaction`

```csharp
public class Goal
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    // Equality basata su Id
    public override bool Equals(object? obj)
    {
        if (obj is not Goal other) return false;
        return Id == other.Id;
    }
}
```

### Value Objects
**Caratteristiche**:
- **NO identità** (definiti dal valore)
- **Immutabili**
- **Uguaglianza** basata su tutti i campi

**Esempi in GoalGrow**:
- `Money`, `DateRange`, `ContactInfo`, `Rating`

```csharp
public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "EUR";
    
    public Money(decimal amount, string currency = "EUR")
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative");
        
        Amount = amount;
        Currency = currency;
    }
    
    // Operatori
    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        
        return new Money(a.Amount + b.Amount, a.Currency);
    }
}
```

---

## Ubiquitous Language

### Terminologia Domain (Glossario)

| Termine | Definizione | Contesto |
|---------|-------------|----------|
| **Goal** | Obiettivo di risparmio con target monetario e deadline | Financial Management |
| **Emergency Fund** | Goal di sistema per fondo emergenze (sempre svincolabile) | Financial Management |
| **Investment Fund** | Goal di sistema che sblocca marketplace a soglia €5k | Financial Management |
| **Portfolio** | Contenitore di investimenti per un investitore | Investment System |
| **Investment** | Singolo investimento in un prodotto finanziario | Investment System |
| **Risk Profile** | Profilo di rischio dell'investitore (conservative ? aggressive) | Compliance |
| **KYC** | Know Your Customer - verifica identità | Compliance |
| **Platform Fee** | Fee 1% (min €1) su transazioni | Financial Management |
| **Badge** | Riconoscimento gamification per azioni compiute | Gamification |
| **Challenge** | Sfida temporizzata con reward monetario | Gamification |
| **Consultant** | Professionista finanziario certificato OCF | Marketplace |
| **Matching** | Abbinamento investitore-consulente | Marketplace |

---

## Domain Services

### Quando usare Domain Service?

Quando la logica **non appartiene** a nessuna entità specifica.

#### Esempio: `GoalUnlockService`

```csharp
public class GoalUnlockService
{
    public bool CanUnlockMarketplace(InversotorUser investor, IEnumerable<Goal> goals)
    {
        // Logica che coinvolge più entità
        var investmentGoal = goals.FirstOrDefault(g => 
            g.Type == GoalType.Investment && g.IsSystemGoal);
        
        if (investmentGoal == null)
            return false;
        
        // Verifica KYC
        if (investor.KycVerification?.Status != KycStatus.Verified)
            return false;
        
        // Verifica soglia
        return investmentGoal.CurrentAmount >= investmentGoal.UnlockThreshold;
    }
}
```

---

## Repositories

### Contratti (Interfaces)

```csharp
public interface IGoalRepository
{
    Task<Goal?> GetByIdAsync(Guid id);
    Task<List<Goal>> GetUserGoalsAsync(Guid userId);
    Task<List<Goal>> GetSystemGoalsAsync(Guid userId);
    Task<Goal> AddAsync(Goal goal);
    Task UpdateAsync(Goal goal);
    Task DeleteAsync(Guid id);
}
```

### Implementazione (Infrastructure Layer)

```csharp
public class GoalRepository : IGoalRepository
{
    private readonly GoalGrowDbContext _context;
    
    public async Task<List<Goal>> GetUserGoalsAsync(Guid userId)
    {
        return await _context.Goals
            .Where(g => g.UserId == userId)
            .Include(g => g.Transactions)
            .OrderByDescending(g => g.Priority)
            .ToListAsync();
    }
}
```

---

## Domain Events *(Planned)*

### Definizione Event

```csharp
public record GoalCompletedEvent(
    Guid GoalId,
    Guid UserId,
    string GoalName,
    decimal TargetAmount,
    DateTime CompletedAt
) : IDomainEvent;
```

### Event Handler

```csharp
public class GoalCompletedEventHandler : IEventHandler<GoalCompletedEvent>
{
    public async Task Handle(GoalCompletedEvent @event)
    {
        // Award badge "FIRST_GOAL"
        // Send notification
        // Update user level XP
        // Create system transaction (reward)
    }
}
```

---

## Anti-Pattern da Evitare

### ? Anemic Domain Model

```csharp
// SBAGLIATO: Entity senza comportamenti
public class Goal
{
    public Guid Id { get; set; }
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    // Solo getter/setter, NO business logic!
}

// Service fa TUTTO
public class GoalService
{
    public void Deposit(Goal goal, decimal amount)
    {
        // Business logic nel service invece che nell'entity
        if (goal.CurrentAmount + amount >= goal.TargetAmount)
        {
            goal.Status = GoalStatus.Completed;
        }
        goal.CurrentAmount += amount;
    }
}
```

### ? Rich Domain Model

```csharp
// CORRETTO: Entity con comportamenti
public class Goal
{
    public void Deposit(decimal amount)
    {
        // Business logic NELL'entity
        ValidateDeposit(amount);
        CurrentAmount += amount;
        CheckCompletion();
    }
    
    private void CheckCompletion()
    {
        if (CurrentAmount >= TargetAmount)
        {
            Complete();
        }
    }
}
```

---

## Roadmap DDD

### Current State ?
- [x] Entities con business logic
- [x] Value Objects (Money, DateRange, Rating)
- [x] Aggregates identificati
- [x] Ubiquitous Language definito

### Next Steps ??
- [ ] Repository pattern interfaces
- [ ] Domain Services
- [ ] Domain Events
- [ ] Specifications Pattern
- [ ] Unit Tests domain logic

### Future ??
- [ ] Event Sourcing
- [ ] CQRS
- [ ] Saga Pattern (transazioni distribuite)

---

## Risorse

- [Domain-Driven Design (Eric Evans)](https://www.domainlanguage.com/ddd/)
- [Implementing DDD (Vaughn Vernon)](https://vaughnvernon.com/)
- [.NET Microservices Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)

---

**Autore**: Edoardo Carollo  
**Data**: 2025-01-18  
**Versione**: 1.0.0
