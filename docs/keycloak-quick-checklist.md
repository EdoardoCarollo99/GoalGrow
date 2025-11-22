# ?? Keycloak Quick Checklist - GoalGrow

## ?? **Checklist Rapida Pre-API**

### ? **1. Docker & Keycloak**
```bash
# Verifica Docker
docker ps | findstr keycloak

# Se non running
docker-compose up -d

# Accesso Admin Console
# URL: http://localhost:8080/admin
# User: admin / Pass: admin
```

### ? **2. Realm: GoalGrow-Dev**
- [ ] Realm creato
- [ ] Email as username: ON
- [ ] User registration: ON

### ? **3. Client: goalgrow-api**
- [ ] Client ID: `goalgrow-api`
- [ ] Client authentication: ON (Confidential)
- [ ] Standard flow: ON
- [ ] Direct access grants: ON
- [ ] Service accounts: ON
- [ ] Valid Redirect URIs: `https://localhost:7001/*`
- [ ] Web origins: `https://localhost:7001`
- [ ] **Client Secret copiato**

### ? **4. Roles**
- [ ] `investor`
- [ ] `consultant`
- [ ] `admin`
- [ ] `kyc-verified`

### ? **5. Test Users**

| Username | Password | Role |
|----------|----------|------|
| admin@goalgrow.com | Admin123! | admin |
| investor@goalgrow.com | Investor123! | investor |
| consultant@goalgrow.com | Consultant123! | consultant |

### ? **6. User Secrets (GoalGrow.API)**
```bash
cd GoalGrow.API

# Init
dotnet user-secrets init

# Authority
dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080/realms/GoalGrow-Dev"

# Client Secret (sostituisci con il tuo)
dotnet user-secrets set "Keycloak:ClientSecret" "YOUR_CLIENT_SECRET_HERE"

# Connection String
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# Verifica
dotnet user-secrets list
```

### ? **7. Test Connessione**
```powershell
# Test OpenID Config
Invoke-WebRequest "http://localhost:8080/realms/GoalGrow-Dev/.well-known/openid-configuration"

# Test Login
$body = @{
    grant_type = "password"
    client_id = "goalgrow-api"
    client_secret = "YOUR_CLIENT_SECRET"
    username = "admin@goalgrow.com"
    password = "Admin123!"
}

$response = Invoke-RestMethod -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" -Method Post -Body $body -ContentType "application/x-www-form-urlencoded"

# Mostra token
$response.access_token
```

### ? **8. Verifica API**
```bash
# Build
dotnet build

# Run API
cd GoalGrow.API
dotnet run

# Test endpoint (con token dal test sopra)
Invoke-RestMethod -Uri "https://localhost:7001/api/auth/verify" -Headers @{Authorization="Bearer YOUR_TOKEN"}
```

---

## ?? **Script Automatico**

Esegui lo script di verifica automatico:

```powershell
.\Test-KeycloakSetup.ps1
```

Questo script verifica:
- ? Docker running
- ? Keycloak raggiungibile
- ? Realm configurato
- ? User Secrets
- ? Database
- ? Build
- ? Login test
- ? API configuration

---

## ?? **Checklist Completa**

Per la checklist dettagliata, vedi:
```
docs/keycloak-pre-api-checklist.md
```

---

## ?? **Troubleshooting Rapido**

| Problema | Soluzione |
|----------|-----------|
| Docker container not found | `docker-compose up -d` |
| Keycloak unreachable | Verifica porta 8080 non occupata |
| Invalid client credentials | Ricontrolla Client Secret |
| Audience validation failed | Audience deve essere `goalgrow-api` |
| Issuer validation failed | Authority: `http://localhost:8080/realms/GoalGrow-Dev` |

---

## ? **Pronto per API?**

Se tutti i check sopra sono ?, sei pronto per:

1. **Implementare UserService**
   - Sync utenti Keycloak ? Database
   - Gestione claim JWT

2. **Creare UsersController**
   - GET /api/users/me
   - PUT /api/users/me

3. **Test con Postman**
   - Login ? Token ? API call

**Next:** Implementazione API Layer ??
