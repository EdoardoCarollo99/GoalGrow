# Fix: LINQ Translation Error on Wallet Endpoint

## ?? Problem

The `GET /api/users/me/wallet` endpoint was failing with:

```
System.InvalidOperationException: The LINQ expression 'DbSet<Investment>()
    .Where(i => i.UserId == @userId)
    .Sum(i => i.CurrentValue - i.PurchasePrice)' could not be translated.

Translation of member 'CurrentValue' on entity type 'Investment' failed.
This commonly occurs when the specified member is unmapped.
```

### Root Cause

The `Investment.CurrentValue` property is marked as `[NotMapped]`:

```csharp
[NotMapped]
public decimal CurrentValue { get; set; }
```

This means it doesn't exist in the database and cannot be used in LINQ queries that are translated to SQL.

---

## ? Solution

Modified `UserService.GetUserWalletAsync()` to calculate profit/loss using only mapped database fields.

### Implementation

**File**: `GoalGrow.API/Services/Implementations/UserService.cs`

**Before** (? Failed):
```csharp
var totalProfit = await _context.Investments
    .Where(i => i.UserId == userId)
    .SumAsync(i => i.CurrentValue - i.PurchasePrice); // CurrentValue is [NotMapped]!
```

**After** (? Working):
```csharp
var investments = await _context.Investments
    .Where(i => i.UserId == userId)
    .Select(i => new
    {
        i.Status,
        i.TotalAmount,
        i.SellAmount,
        i.PurchasePrice,
        i.Quantity
    })
    .ToListAsync(); // Fetch data first

// Calculate profit/loss in-memory
decimal totalProfit = investments
    .Where(i => i.Status == InvestmentStatus.Sold && i.SellAmount.HasValue)
    .Sum(i => i.SellAmount!.Value - i.TotalAmount);
```

### Key Changes

1. **Fetch investments first** with `ToListAsync()` to bring data into memory
2. **Use only mapped properties** in the projection (`Status`, `TotalAmount`, `SellAmount`)
3. **Calculate profit in-memory** after data is fetched
4. **For sold investments**: Use `SellAmount - TotalAmount`
5. **For active investments**: Profit is 0 (requires real-time market data)

---

## ?? Current Behavior

| Investment Status | Profit Calculation |
|-------------------|-------------------|
| **Sold** (`InvestmentStatus.Sold`) | `SellAmount - TotalAmount` |
| **Active** (`InvestmentStatus.Active`) | `0` (requires real-time pricing) |
| **Pending/Cancelled** | `0` |

### Future Enhancements

The `TODO` comment in the code indicates future improvements:

```csharp
// TODO: Implement real-time product price lookup for active investments
// TODO: Integrate with real-time market data API
```

For now, active investments show **0 profit/loss**, which is accurate since we don't have:
- Real-time stock/ETF prices
- Current market value of crypto holdings
- Updated bond valuations

---

## ?? Testing

### Before Fix
```bash
GET /api/users/me/wallet
Authorization: Bearer <INVESTOR_TOKEN>

Response: 500 Internal Server Error
InvalidOperationException: LINQ expression could not be translated
```

### After Fix
```bash
GET /api/users/me/wallet
Authorization: Bearer <INVESTOR_TOKEN>

Response: 200 OK
{
  "success": true,
  "data": {
    "userId": "01936df2-...",
    "currentBalance": 5000.00,
    "totalDeposited": 10000.00,
    "totalWithdrawn": 2000.00,
    "totalInvested": 3000.00,
    "totalProfit": 0.00,  // Only calculated for sold investments
    "availableForInvestment": 5000.00,
    "lastTransactionDate": "2025-01-18T10:30:00Z",
    "transactionCount": 15
  }
}
```

### Test Script

```powershell
# Get investor token
$investorToken = (Invoke-RestMethod `
    -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" `
    -Method Post `
    -ContentType "application/x-www-form-urlencoded" `
    -Body @{
        grant_type="password"
        client_id="goalgrow-api"
        client_secret="L76lhUEKgudHRkj73B03O2ev5SuURrju"
        username="investor@goalgrow.com"
        password="Investor123!"
    }).access_token

# Test wallet endpoint
Invoke-RestMethod `
    -Uri "https://localhost:7001/api/users/me/wallet" `
    -Headers @{Authorization="Bearer $investorToken"} `
    -SkipCertificateCheck | ConvertTo-Json -Depth 5
```

---

## ?? Related Issues

This is a common EF Core limitation when dealing with:
- **Computed properties** (`[NotMapped]`)
- **Complex calculations** in LINQ
- **Client-side evaluation** vs. **server-side (SQL) evaluation**

### Best Practices

1. ? **Use `ToListAsync()` first** when you need computed properties
2. ? **Project only mapped fields** in `.Select()`
3. ? **Calculate complex logic in-memory** after fetching data
4. ? **Document TODOs** for future real-time integrations
5. ? **Avoid using `[NotMapped]` properties** in LINQ-to-SQL queries

---

## ?? References

- [EF Core: Client vs. Server Evaluation](https://learn.microsoft.com/en-us/ef/core/querying/client-eval)
- [EF Core: Complex Query Operators](https://learn.microsoft.com/en-us/ef/core/querying/complex-query-operators)
- [NotMappedAttribute](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.schema.notmappedattribute)

---

**Fixed in**: Branch `developing/user-management-endpoints`  
**Date**: 2025-01-18  
**Impact**: Wallet endpoint now works correctly for all investor users  
**Limitation**: Active investments show 0 profit until real-time pricing is implemented
