# ?? GoalGrow - Setup Rapido

## ? Cosa è stato fatto

### 1. **AdminUser** aggiunto
- ? Modello `AdminUser` creato in `GoalGrow.Entity/Models/`
- ? Configurazione EF Core in `GoalGrow.Data/Configurations/`
- ? Discriminator TPH aggiornato per supportare Admin, Investor, Consultant

### 2. **DatabaseSeeder** sistemato
- ? Rimossi dati di test complessi
- ? Dati essenziali:
  - 1 Admin (`admin@goalgrow.com`)
  - 1 Consultant (`consultant@goalgrow.com`)
  - 1 Investor (`investor@goalgrow.com`)
  - 4 Badge fondamentali
  - 2 Challenge base
  - 3 Prodotti finanziari (VWCE ETF, AAPL, BTP)
  - 2 Goal di sistema (Emergency + Investment Fund)

---

## ??? Setup Database

### Opzione A: Script Automatico (Consigliato)

```powershell
.\Setup-Database.ps1
```

Questo script:
1. Crea migration `AddAdminUser` e `OptimizeIndexesAndForeignKeys`
2. Applica migration al database
3. Esegue il seeding con dati essenziali

### Opzione B: Manuale

```powershell
cd GoalGrow.Migration

# 1. Crea migration AdminUser
dotnet ef migrations add AddAdminUser --project ../GoalGrow.Data --startup-project .

# 2. Crea migration Indici Ottimizzati
dotnet ef migrations add OptimizeIndexesAndForeignKeys --project ../GoalGrow.Data --startup-project .

# 3. Applica migration
dotnet ef database update --project ../GoalGrow.Data --startup-project .

# 4. Seed database
dotnet run
```

---

## ?? Utenti Creati

| Email | Tipo | Password (Keycloak) | Ruolo |
|-------|------|---------------------|-------|
| `admin@goalgrow.com` | Admin | `Admin123!` | SuperAdmin |
| `consultant@goalgrow.com` | Consultant | `Consultant123!` | consultant, kyc-verified |
| `investor@goalgrow.com` | Investor | `Investor123!` | investor |

---

## ?? Setup Keycloak

### 1. Avvia Keycloak

```powershell
docker-compose up -d
```

Accedi a: http://localhost:8080
- Username: `admin`
- Password: `admin`

### 2. Crea Utenti in Keycloak

**Realm:** `GoalGrowe` (già esistente)

#### Admin User
```
Email: admin@goalgrow.com
Username: admin
Password: Admin123!
Roles: admin, kyc-verified
```

#### Consultant User
```
Email: consultant@goalgrow.com
Username: consultant
Password: Consultant123!
Roles: consultant, kyc-verified
```

#### Investor User
```
Email: investor@goalgrow.com
Username: investor
Password: Investor123!
Roles: investor
```

### 3. Configura Client `goalgrow-api`

- Client ID: `goalgrow-api`
- Client Protocol: `openid-connect`
- Access Type: `confidential`
- Valid Redirect URIs: `https://localhost:5001/*`
- Web Origins: `https://localhost:5001`

**Importante:** Copia il **Client Secret** da:
```
Clients ? goalgrow-api ? Credentials ? Secret
```

---

## ?? Avvia API

### 1. Configura User Secrets

```powershell
cd GoalGrow.API

dotenv user-secrets set "Keycloak:Authority" "http://localhost:8080/realms/GoalGrowe"
dotenv user-secrets set "Keycloak:ClientSecret" "<COPIA-IL-SECRET-DA-KEYCLOAK>"
dotenv user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"
```

### 2. Avvia l'API

```powershell
dotnet run
```

Accedi a: https://localhost:5001/scalar

---

## ?? Test Login

```powershell
# Login come Investor
$loginBody = @{
    username = "investor"
    password = "Investor123!"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:5001/api/auth/login" `
    -Method POST `
    -Body $loginBody `
    -ContentType "application/json"

# Usa il token
$token = $response.data.accessToken
$headers = @{ Authorization = "Bearer $token" }

Invoke-RestMethod -Uri "https://localhost:5001/api/health/secure" -Headers $headers
```

---

## ?? Struttura Database

### Tabelle Principali

| Tabella | Descrizione |
|---------|-------------|
| `Users` | Tabella TPH con Admin, Consultant, Investor |
| `Accounts` | Conti correnti degli utenti |
| `Goals` | Obiettivi di risparmio (Emergency, Investment, Custom) |
| `InvestmentProducts` | Prodotti finanziari disponibili |
| `Investments` | Investimenti effettuati |
| `KycVerifications` | Verifiche KYC |
| `PlatformFees` | Fee del 1% su transazioni |
| `Badges` | Badge gamification |
| `Challenges` | Sfide con reward |

---

## ?? Prossimi Step

1. ? **Setup completato** - Database e Keycloak pronti
2. ?? **Sviluppo API** - Implementare endpoint:
   - `/api/users/me` - Profilo utente corrente
   - `/api/goals` - CRUD obiettivi
   - `/api/investments` - CRUD investimenti
   - `/api/consultants` - Marketplace consulenti
3. ?? **Frontend** - Blazor Web App
4. ?? **Mobile** - MAUI App

---

## ?? Note

- **Admin** ha accesso a tutto il sistema (gestione utenti, fee, report)
- **Consultant** può gestire clienti e commissioni
- **Investor** ha wallet, goals, investments, KYC

---

**Autore:** Edoardo Carollo  
**Versione:** 1.0.0  
**Data:** 2025-01-18
