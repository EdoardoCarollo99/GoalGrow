# Goals Authorization & Privacy Rules

## ?? Security & Privacy Model

### Overview

Goals are **strictly private** to investors. Only the investor who owns a goal can view, modify, or manage it. Consultants and admins have limited access based on business requirements.

---

## ?? Access Control by Role

### 1. **Investor Role** 

? **Full Access to Own Goals**
- Create new savings goals
- View all own goals (list & details)
- Update goal properties
- Contribute to goals (from virtual wallet)
- Withdraw from goals (if not locked)
- Pause/resume goals
- Complete goals manually
- Delete goals (soft delete)
- View progress & milestones

? **Cannot**
- View other investors' goals
- Access goals statistics

**Endpoints Accessible:**
- `GET /api/goals` - List own goals
- `GET /api/goals/{id}` - View specific goal
- `POST /api/goals` - Create new goal
- `PUT /api/goals/{id}` - Update goal
- `DELETE /api/goals/{id}` - Delete goal
- `GET /api/goals/{id}/progress` - View progress
- `POST /api/goals/{id}/contribute` - Add funds
- `POST /api/goals/{id}/withdraw` - Withdraw funds
- `POST /api/goals/{id}/pause` - Pause goal
- `POST /api/goals/{id}/resume` - Resume goal
- `POST /api/goals/{id}/complete` - Mark completed

---

### 2. **Consultant Role**

? **NO Direct Access to Goals**

Consultants **cannot** view investor goals for privacy reasons. They only have access to:

? **Limited Financial Information via User Wallet:**
- `GET /api/users/me/wallet` - View client's wallet balance
- See total invested amount
- See funds available for investment

? **Investment Fund Goal (Special Case)**
- Can see if client has reached the Investment Fund unlock threshold
- Can see `HasReachedUnlockThreshold` flag
- **Cannot** see goal details (name, target, current amount)

**Why?**
- Goals are personal savings objectives (vacations, emergency funds, etc.)
- Consultants only need to know available investment capital
- Privacy-first approach

**Implementation:**
- Consultants should use `GET /api/users/{clientId}/wallet` endpoint
- Check `AvailableForInvestment` field
- No need to access `/api/goals` at all

---

### 3. **Admin Role**

? **Platform-Wide Statistics Only**
- `GET /api/goals/stats` - Aggregate statistics

**Admin Can See:**
- Total number of goals (by status, type)
- Total amount saved across all users
- Average goal size
- Completion rates
- Goals completed this month

? **Admin Cannot See:**
- Individual user's goals
- Personal goal details
- User-identifiable information in goals

**Why?**
- Admins manage the platform, not user data
- Privacy compliance (GDPR)
- Statistics are anonymized

---

## ?? Authorization Implementation

### Endpoint-Level Security

All goal endpoints require `[Authorize]` attribute with specific roles:

```csharp
[Authorize(Roles = "investor")]  // Only investors
public async Task<IActionResult> GetMyGoals()

[Authorize(Roles = "admin")]     // Only admins
public async Task<IActionResult> GetGoalStats()
```

### Service-Level Security

`GoalService` enforces **UserId matching**:

```csharp
public async Task<GoalResponse?> GetGoalByIdAsync(Guid goalId, Guid userId)
{
    // ALWAYS filter by userId - prevents unauthorized access
    var goal = await _context.Goals
        .FirstOrDefaultAsync(g => g.Id == goalId && g.UserId == userId);
    
    if (goal == null)
        return null;  // Goal not found OR user doesn't own it
    
    return MapToGoalResponse(goal);
}
```

**Key Points:**
- Even if a malicious user knows another user's GoalId
- They cannot access it because the query filters by `UserId`
- SQL injection prevented by EF Core parameterization

---

## ?? Data Visibility Matrix

| Data | Investor (Owner) | Consultant | Admin |
|------|------------------|------------|-------|
| **Goal Details** | ? Full | ? None | ? None |
| **Goal Progress** | ? Yes | ? No | ? No |
| **Goal Transactions** | ? Yes | ? No | ? No |
| **Virtual Wallet Balance** | ? Yes | ? Yes (client only) | ? No |
| **Investment Fund Threshold** | ? Yes | ? Yes (flag only) | ? No |
| **Platform Statistics** | ? No | ? No | ? Yes |

---

## ??? Privacy-First Design

### Principles

1. **Least Privilege**
   - Users only see what they need
   - Consultants don't need goal details to do their job

2. **Data Minimization**
   - Consultants get only `AvailableForInvestment` amount
   - No personal goal names, descriptions, or categories

3. **Explicit Consent**
   - If in future we add "share goal with consultant" feature
   - It will require explicit investor approval

4. **Audit Trail**
   - All goal access is logged
   - Admin operations are tracked

---

## ?? Future Enhancements

### Potential Features (Not Implemented)

1. **Share Goal with Consultant** (Optional)
   ```csharp
   POST /api/goals/{id}/share
   Body: { consultantId: "guid", permissions: ["view"] }
   ```

2. **Goal Visibility Levels**
   - Private (default)
   - Shared with consultant
   - Public (for gamification leaderboards)

3. **Consultant Dashboard**
   - See aggregate client savings rate
   - See total portfolio value across all clients
   - **Still no individual goal access**

---

## ?? Implementation Checklist

- [x] All goal endpoints require authentication
- [x] Investor-only endpoints marked with `[Authorize(Roles = "investor")]`
- [x] Admin stats endpoint marked with `[Authorize(Roles = "admin")]`
- [x] Service layer enforces `UserId` filtering
- [x] No consultant access to goals
- [x] Logging implemented for audit trail
- [x] Error messages don't leak information (generic "not found")

---

## ?? Testing Security

### Test Cases

1. **Investor Access**
   ```powershell
   # Should succeed
   GET /api/goals
   Authorization: Bearer <INVESTOR_TOKEN>
   ```

2. **Consultant Blocked**
   ```powershell
   # Should return 403 Forbidden
   GET /api/goals
   Authorization: Bearer <CONSULTANT_TOKEN>
   ```

3. **Cross-User Access**
   ```powershell
   # Should return 404 (not 403, to prevent enumeration)
   GET /api/goals/{another-user-goal-id}
   Authorization: Bearer <INVESTOR_TOKEN>
   ```

4. **Admin Statistics**
   ```powershell
   # Should succeed
   GET /api/goals/stats
   Authorization: Bearer <ADMIN_TOKEN>
   ```

---

## ?? Related Documentation

- [User Management Authorization](USER_MANAGEMENT_ENDPOINTS.md#security--authorization)
- [Wallet Endpoint Access](USER_MANAGEMENT_ENDPOINTS.md#wallet-management-investor-only)
- [GDPR Compliance](../GDPR_COMPLIANCE.md)

---

**Last Updated**: 2025-01-18  
**Version**: 1.0  
**Status**: ? Implemented and Enforced
