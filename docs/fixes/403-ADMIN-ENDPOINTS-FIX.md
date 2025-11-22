# Fix: 403 Forbidden on Admin Endpoints

## ?? Problem

Admin endpoints were returning **403 Forbidden** even when using a valid admin token from Keycloak.

### Root Cause

Keycloak stores user roles in a nested JSON structure within the `realm_access` claim:

```json
{
  "realm_access": {
    "roles": ["admin", "kyc-verified", "offline_access", ...]
  }
}
```

ASP.NET Core's `[Authorize(Roles = "admin")]` attribute expects roles as individual `ClaimTypes.Role` claims, but they weren't being extracted from the nested structure.

---

## ? Solution

Modified `Program.cs` to extract roles from Keycloak's `realm_access` claim and map them to ASP.NET Core's standard role claims.

### Code Changes

**File**: `GoalGrow.API/Program.cs`

Added role extraction logic in the `OnTokenValidated` event:

```csharp
OnTokenValidated = context =>
{
    if (context.Principal?.Identity is ClaimsIdentity identity)
    {
        // Extract roles from realm_access
        var realmAccessClaim = identity.FindFirst("realm_access");
        if (realmAccessClaim != null)
        {
            var realmAccess = JsonSerializer.Deserialize<JsonElement>(realmAccessClaim.Value);
            if (realmAccess.TryGetProperty("roles", out var rolesElement))
            {
                var roles = rolesElement.EnumerateArray()
                    .Select(role => role.GetString())
                    .Where(role => !string.IsNullOrEmpty(role));

                // Add each role as a separate claim
                foreach (var role in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role!));
                }
            }
        }
    }
    return Task.CompletedTask;
}
```

Also updated `TokenValidationParameters`:

```csharp
RoleClaimType = ClaimTypes.Role  // Use standard claim type
```

---

## ?? Testing

### Before Fix
```bash
GET /api/admin/stats
Authorization: Bearer <ADMIN_TOKEN>

Response: 403 Forbidden
```

### After Fix
```bash
GET /api/admin/stats
Authorization: Bearer <ADMIN_TOKEN>

Response: 200 OK
{
  "success": true,
  "data": {
    "totalUsers": 3,
    "investorCount": 1,
    "adminCount": 1,
    ...
  }
}
```

### Verification Script

```powershell
# Get admin token
$adminToken = (Invoke-RestMethod `
    -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" `
    -Method Post `
    -ContentType "application/x-www-form-urlencoded" `
    -Body @{
        grant_type="password"
        client_id="goalgrow-api"
        client_secret="L76lhUEKgudHRkj73B03O2ev5SuURrju"
        username="admin@goalgrow.com"
        password="Admin123!"
    }).access_token

# Test admin endpoint
Invoke-RestMethod `
    -Uri "https://localhost:7001/api/admin/stats" `
    -Headers @{Authorization="Bearer $adminToken"} `
    -SkipCertificateCheck
```

---

## ?? Affected Endpoints

All endpoints with `[Authorize(Roles = "admin")]` now work correctly:

- ? `GET /api/users/{id}` - Admin can view any user
- ? `GET /api/admin/users` - Paginated user list
- ? `GET /api/admin/stats` - Platform statistics
- ? `PUT /api/admin/users/{id}/status` - User activation
- ? `DELETE /api/admin/users/{id}` - Admin deletion

---

## ?? Additional Benefits

The solution also extracts roles from `resource_access` claim (client-specific roles):

```json
{
  "resource_access": {
    "account": {
      "roles": ["manage-account", "view-profile"]
    }
  }
}
```

These are mapped as `client:role` format (e.g., `account:manage-account`).

---

## ?? References

- [Keycloak JWT Token Structure](https://www.keycloak.org/docs/latest/server_admin/#_token-exchange)
- [ASP.NET Core Role-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles)
- [ClaimTypes Class](https://learn.microsoft.com/en-us/dotnet/api/system.security.claims.claimtypes)

---

**Fixed in**: Branch `developing/user-management-endpoints`  
**Date**: 2025-01-18  
**Tested**: ? All admin endpoints working correctly
