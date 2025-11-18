# GoalGrow API - Quick Start

## Cosa Hai Fatto Finora

-  Progetto GoalGrow.API creato
-  Pacchetti NuGet installati (JWT, EF Core, AutoMapper, Serilog)
-  Riferimenti ai progetti GoalGrow.Data e GoalGrow.Entity aggiunti
-  Struttura cartelle creata (Controllers, Services, DTOs, Extensions)
-  appsettings.json configurato
-  Program.cs con JWT Authentication
-  HealthController di test creato
-  Build successful 

---

##  Setup User Secrets

### Opzione A: Script Automatico (Consigliato)

```powershell
.\Setup-API.ps1
```

Ti chiederà:
- Keycloak Authority URL
- Keycloak Client Secret

### Opzione B: Manuale

```powershell
cd GoalGrow.API

dotnet user-secrets init

# Connection String
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# Keycloak (SOSTITUISCI con i tuoi valori)
dotnet user-secrets set "Keycloak:Authority" "https://your-keycloak.com/realms/GoalGrow-Dev"
dotnet user-secrets set "Keycloak:ClientSecret" "your-client-secret"

# Verifica
dotnet user-secrets list
```

---

##  Avvia l'API

```powershell
cd GoalGrow.API
dotnet run
```

**Output atteso:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
🚀 GoalGrow API started on https://localhost:5001
```

---

##  Test Endpoints

### 1. Health Check (No Auth)

```bash
GET https://localhost:5001/api/health
```

**Response:**
```json
{
  "success": true,
  "message": "API is running",
  "data": {
    "status": "Healthy",
    "timestamp": "2025-01-18T14:30:00Z",
    "version": "1.0.0"
  },
  "timestamp": "2025-01-18T14:30:00Z"
}
```

### 2. Secure Endpoint (Requires JWT)

**Step 1: Get JWT Token from Keycloak**

```bash
curl -X POST 'https://your-keycloak.com/realms/GoalGrow-Dev/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'grant_type=password' \
  -d 'client_id=goalgrow-api' \
  -d 'client_secret=YOUR-CLIENT-SECRET' \
  -d 'username=investor' \
  -d 'password=Password123!'
```

**Response:**
```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 300,
  "refresh_token": "...",
  "token_type": "Bearer"
}
```

**Step 2: Call Secure Endpoint**

```bash
GET https://localhost:5001/api/health/secure
Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response:**
```json
{
  "success": true,
  "data": {
    "message": "You are authenticated!",
    "userId": "f:keycloak-uuid:...",
    "email": "investor@test.com",
    "claims": [
      { "type": "sub", "value": "..." },
      { "type": "email", "value": "investor@test.com" },
      { "type": "realm_access.roles", "value": "investor" }
    ]
  }
}
```

---

## Swagger UI

Apri browser: **https://localhost:5001/swagger**

- Clicca "Authorize"
- Incolla il JWT token (senza "Bearer ")
- Testa gli endpoint

---

## Struttura Attuale

```
GoalGrow.API/
├── Controllers/
│   └── HealthController.cs         ✅ Test controller
├── Services/
│   ├── Interfaces/                 (vuoto per ora)
│   └── Implementations/            (vuoto per ora)
├── DTOs/
│   ├── Requests/
│   │   └── UpdateProfileRequest.cs ✅
│   └── Responses/
│       ├── ApiResponse.cs          ✅ Response wrapper
│       └── UserResponse.cs         ✅ User DTOs
├── Extensions/
│   └── ServiceCollectionExtensions.cs ✅
├── Program.cs                       ✅ JWT Auth configurato
└── appsettings.json                 ✅ Configurazione completa
```

---

## Prossimi Passi

### Step 1: Crea UserService

**File da creare:**
- `Services/Interfaces/IUserService.cs`
- `Services/Implementations/UserService.cs`

**Cosa fa:**
- Ottiene utente da JWT claims
- Crea utente se non esiste nel DB
- Sincronizza dati Keycloak ↔ Database

### Step 2: Crea UsersController

**File da creare:**
- `Controllers/UsersController.cs`

**Endpoint:**
- `GET /api/users/me` - Current user info
- `PUT /api/users/me` - Update profile

### Step 3: Test con Postman

**Collection:**
1. Get JWT Token
2. GET /api/users/me
3. PUT /api/users/me

---

## Checklist Completamento

### Da Fare 🚧
- [ ] User Secrets configurati (esegui `Setup-API.ps1`)
- [ ] UserService implementato
- [ ] UsersController implementato
- [ ] Test con Postman/cURL
- [ ] Documentazione API completa

---

## Troubleshooting

### Errore: "Unable to obtain configuration"

**Causa:** Keycloak Authority non raggiungibile

**Soluzione:**
1. Verifica che Keycloak sia avviato
2. Controlla URL in User Secrets: `dotnet user-secrets list`
3. Test URL: `curl https://your-keycloak.com/realms/GoalGrow-Dev/.well-known/openid-configuration`

### Errore: "Cannot open database"

**Causa:** Connection string errata o SQL Server non in esecuzione

**Soluzione:**
1. Verifica User Secrets: `dotnet user-secrets list`
2. Test connessione: Apri SSMS e connettiti a `Server=.;Database=GoalGrowDb`

---

## Documentazione

- [Keycloak Integration Guide](../docs/technical/AUTHENTICATION.md)
- [API Architecture](../docs/technical/ARCHITECTURE.md)
- [Task List](../TASK_LIST.md)