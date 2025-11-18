# Authentication & Authorization Architecture

## Overview

GoalGrow utilizza un'architettura di autenticazione moderna basata su **OpenID Connect** con provider esterni (Keycloak, Auth0, Azure AD B2C).

---

## ?? Authentication Strategy

### Technology Stack
- **Protocol**: OpenID Connect (OIDC) + OAuth 2.0
- **Tokens**: JWT (JSON Web Tokens)
- **Identity Provider**: Keycloak (self-hosted) o Auth0 (managed)
- **.NET Integration**: Microsoft.AspNetCore.Authentication.OpenIdConnect

### User Types & Roles

```
???????????????????????????????????????
?      Identity Provider (Keycloak)   ?
?                                     ?
?  Realms:                            ?
?  ??? GoalGrow-Production           ?
?  ?   ??? Users (InvestorUser)     ?
?  ?   ??? Consultants              ?
?  ?   ??? Admins                   ?
?  ?                                 ?
?  ??? Roles:                        ?
?      ??? investor                  ?
?      ??? consultant                ?
?      ??? admin                     ?
?      ??? kyc-verified              ?
???????????????????????????????????????
              ?
???????????????????????????????????????
?         GoalGrow API                ?
?  (ASP.NET Core with JWT Validation) ?
???????????????????????????????????????
```

---

## ?? Required NuGet Packages

### GoalGrow.Api (to be created)
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="10.0.0" />
<PackageReference Include="IdentityModel" Version="7.0.0" />
```

---

## ?? Identity Provider Setup

### Option 1: Keycloak (Self-Hosted, Open Source)

**Advantages:**
- ? Free and open source
- ? Full control over data
- ? EU data residency compliant
- ? Highly customizable
- ? Supports Multi-tenancy

**Setup:**
```yaml
# docker-compose.yml
version: '3.8'
services:
  keycloak:
    image: quay.io/keycloak/keycloak:latest
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: password
    ports:
      - "8080:8080"
    command: start-dev
    depends_on:
      - postgres
      
  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: keycloak
      POSTGRES_USER: keycloak
      POSTGRES_PASSWORD: password
    volumes:
      - keycloak_data:/var/lib/postgresql/data

volumes:
  keycloak_data:
```

**Keycloak Configuration:**
1. Create Realm: `GoalGrow-Production`
2. Create Clients:
   - `goalgrow-api` (Resource Server)
   - `goalgrow-web` (Blazor Web App)
   - `goalgrow-mobile` (Mobile App)
3. Create Roles:
   - `investor`
   - `consultant`
   - `admin`
   - `kyc-verified` (for compliance)
4. Configure User Federation (optional): Connect to LDAP/AD

---

### Option 2: Auth0 (Managed SaaS)

**Advantages:**
- ? No infrastructure management
- ? Built-in MFA, Anomaly Detection
- ? Social logins (Google, Apple, etc.)
- ? Excellent documentation

**Pricing:** Free up to 7,000 active users/month

---

### Option 3: Azure AD B2C

**Advantages:**
- ? Integrated with Azure ecosystem
- ? Compliance certifications (GDPR, SOC 2)
- ? Global scale

**Pricing:** €0.00325 per authentication (first 50K free)

---

## ??? Recommended Choice: **Keycloak**

**Rationale:**
- Full control over user data (GDPR compliance)
- No per-user costs (scalable)
- Can be self-hosted in EU for data residency
- Open source = no vendor lock-in
- Perfect for fintech where data sovereignty matters

---

## ?? Implementation Architecture

### 1. User Registration Flow

```
???????????      ????????????      ?????????????      ???????????????
?  User   ???????? Keycloak ???????? GoalGrow  ????????  Database   ?
? (Blazor)????????  (OIDC)  ????????    API    ???????? (User Table)?
???????????      ????????????      ?????????????      ???????????????
    ?                  ?                  ?
    ?   1. Register    ?                  ?
    ????????????????????                  ?
    ?                  ?                  ?
    ?   2. JWT Token   ?                  ?
    ????????????????????                  ?
    ?                  ?                  ?
    ?   3. Create User Profile           ?
    ???????????????????????????????????????
    ?                  ?                  ?
    ?                  ?   4. Save User   ?
    ?                  ????????????????????
```

**Steps:**
1. User registers via Keycloak (email + password or social login)
2. Keycloak returns JWT token with `sub` (subject ID)
3. Blazor app calls GoalGrow API with JWT
4. API validates JWT and creates `InversotorUser` record
5. Link Keycloak `sub` to `InversotorUser.Id`

---

### 2. JWT Token Structure

```json
{
  "sub": "keycloak-user-id-123",
  "email": "user@example.com",
  "email_verified": true,
  "realm_access": {
    "roles": ["investor", "kyc-verified"]
  },
  "resource_access": {
    "goalgrow-api": {
      "roles": ["user"]
    }
  },
  "preferred_username": "john.doe",
  "given_name": "John",
  "family_name": "Doe",
  "iat": 1700000000,
  "exp": 1700003600
}
```

---

### 3. API Authentication Setup

```csharp
// Program.cs in GoalGrow.Api
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://keycloak.goalgrow.com/realms/GoalGrow-Production";
        options.Audience = "goalgrow-api";
        options.RequireHttpsMetadata = true;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InvestorOnly", policy =>
        policy.RequireRole("investor"));
        
    options.AddPolicy("ConsultantOnly", policy =>
        policy.RequireRole("consultant"));
        
    options.AddPolicy("KycVerified", policy =>
        policy.RequireRole("kyc-verified"));
});
```

---

### 4. Controller Authorization

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requires valid JWT
public class InvestmentController : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Policy = "KycVerified")] // Only KYC-verified users
    public async Task<IActionResult> CreateInvestment([FromBody] CreateInvestmentRequest request)
    {
        var userId = User.FindFirst("sub")?.Value; // Keycloak user ID
        // Create investment...
    }
    
    [HttpGet("consultant/commissions")]
    [Authorize(Policy = "ConsultantOnly")]
    public async Task<IActionResult> GetCommissions()
    {
        // Only consultants can access
    }
}
```

---

## ?? User Linking Strategy

### Database Schema Update

```csharp
// GoalGrow.Entity/Super/User.cs
public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    // NEW: Link to Keycloak user
    [Required]
    [MaxLength(255)]
    public string KeycloakSubjectId { get; set; } = string.Empty; // Keycloak "sub" claim
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    // ... rest
}
```

### User Creation Flow

```csharp
// Service to sync Keycloak user with GoalGrow database
public class UserSyncService
{
    private readonly GoalGrowDbContext _db;
    
    public async Task<InversotorUser> GetOrCreateUserAsync(ClaimsPrincipal principal)
    {
        var keycloakSub = principal.FindFirst("sub")?.Value 
            ?? throw new UnauthorizedAccessException("Invalid token");
        
        // Check if user exists
        var user = await _db.InvestorUsers
            .FirstOrDefaultAsync(u => u.KeycloakSubjectId == keycloakSub);
        
        if (user == null)
        {
            // Create new user from JWT claims
            user = new InversotorUser(
                firstName: principal.FindFirst("given_name")?.Value ?? "",
                lastName: principal.FindFirst("family_name")?.Value ?? "",
                phoneNumber: principal.FindFirst("phone_number")?.Value ?? "",
                emailAddress: principal.FindFirst("email")?.Value ?? "",
                fiscalCode: "", // To be filled later in KYC
                birthDate: DateTime.MinValue // To be filled later
            )
            {
                KeycloakSubjectId = keycloakSub
            };
            
            _db.InvestorUsers.Add(user);
            await _db.SaveChangesAsync();
        }
        
        return user;
    }
}
```

---

## ??? Security Best Practices

### 1. Token Refresh Strategy
- Access Token: Short-lived (15 minutes)
- Refresh Token: Long-lived (30 days)
- Store refresh token in HttpOnly cookie

### 2. HTTPS Only
```csharp
app.UseHttpsRedirection();
app.UseHsts(); // Force HTTPS in production
```

### 3. CORS Configuration
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("https://app.goalgrow.com", "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

### 4. Rate Limiting
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

---

## ?? Multi-Platform Support

### Blazor Web App (OIDC Flow)
```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = "https://keycloak.goalgrow.com/realms/GoalGrow-Production";
    options.ClientId = "goalgrow-web";
    options.ClientSecret = "client-secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
});
```

### Mobile App (PKCE Flow)
- Use [IdentityModel.OidcClient](https://github.com/IdentityModel/IdentityModel.OidcClient)
- PKCE (Proof Key for Code Exchange) for native apps
- Store tokens securely using platform-specific secure storage

---

## ?? Testing

### Unit Tests
```csharp
[Fact]
public async Task CreateInvestment_WithoutKyc_ShouldReturn403()
{
    // Arrange
    var client = _factory.CreateClient();
    var token = GenerateJwtWithoutKycRole();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    
    // Act
    var response = await client.PostAsync("/api/investment/create", content);
    
    // Assert
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
}
```

---

## ?? Resources

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [OIDC Specification](https://openid.net/specs/openid-connect-core-1_0.html)
- [ASP.NET Core Security](https://learn.microsoft.com/en-us/aspnet/core/security/)
- [IdentityModel](https://identitymodel.readthedocs.io/)

---

## ? Migration Checklist

- [ ] Set up Keycloak instance (Docker or managed)
- [ ] Create GoalGrow realm and clients
- [ ] Add `KeycloakSubjectId` to User entity
- [ ] Create migration to add column
- [ ] Implement JWT validation in API
- [ ] Create `UserSyncService`
- [ ] Add authorization policies
- [ ] Configure OIDC in Blazor app
- [ ] Implement mobile PKCE flow
- [ ] Add rate limiting
- [ ] Set up HTTPS certificates
- [ ] Test end-to-end authentication
