# GoalGrow

> Piattaforma Fintech per Gestione Finanziaria Personale, Obiettivi di Risparmio e Marketplace Consulenti

[![.NET](https://img.shields.io/badge/.NET-10-purple)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-14.0-blue)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build](https://img.shields.io/badge/Build-Passing-brightgreen)](CHANGELOG.md)

---

## Panoramica Tecnica

**GoalGrow** è una piattaforma fintech B2C che combina gestione finanziaria personale, obiettivi di risparmio guidati e un marketplace di consulenti finanziari certificati. Il progetto implementa **Clean Architecture** e **Domain-Driven Design** utilizzando le più recenti tecnologie .NET.

### Stack Tecnologico

| Layer | Tecnologia |
|-------|------------|
| **Backend API** | ASP.NET Core 10.0 Web API |
| **Database** | SQL Server + Entity Framework Core 10.0 |
| **Autenticazione** | Keycloak (OpenID Connect / OAuth 2.0) |
| **Frontend** | Blazor Server *(planned)* |
| **Mobile** | .NET MAUI *(planned)* |
| **Storage** | Azure Blob Storage *(KYC documents)* |

### Architettura Soluzione

```
GoalGrow/
??? GoalGrow.Entity/          # Domain Layer (Entities, Enums, Value Objects)
??? GoalGrow.Data/            # Infrastructure Layer (EF Core, Configurations)
??? GoalGrow.Migration/       # Database Management (Migrations, Seeding)
??? GoalGrow.API/             # Presentation Layer (REST API)
??? docs/                     # Documentazione completa
??? tests/                    # Unit & Integration Tests *(planned)*
```

**Pattern Implementati:**
- Clean Architecture
- Domain-Driven Design (DDD)
- Repository Pattern
- CQRS *(planned)*
- Event Sourcing *(planned)*

---

## Quick Start

### Prerequisiti

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/sql-server) (Express o Developer Edition)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (per Keycloak)
- [Visual Studio 2025](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### Setup Database e Seeding

```bash
# 1. Clona repository
git clone https://github.com/EdoardoCarollo99/GoalGrow.git
cd GoalGrow

# 2. Configura connection string
cd GoalGrow.Migration
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# 3. Esegui migration e seeding
dotnet run
```

**Cosa fa:**
- Crea database `GoalGrowDb` con schema ottimizzato
- Applica migration EF Core
- Popola database con dati essenziali (3 utenti, prodotti, badge, challenge)

### Avvio Keycloak

```bash
# Dalla root del progetto
docker-compose up -d
```

Accesso Admin Console: http://localhost:8080 (admin/admin)

### Configurazione API

```bash
cd GoalGrow.API

# Inizializza user secrets
dotnet user-secrets init

# Configura Keycloak
dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080/realms/GoalGrowe"
dotnet user-secrets set "Keycloak:ClientSecret" "<CLIENT_SECRET_DA_KEYCLOAK>"

# Configura database
dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"

# Avvia API
dotnet run
```

API disponibile su: https://localhost:5001/scalar

---

## Funzionalità Principali

### Core Features

- **Smart Savings**: Wallet virtuale con obiettivi di risparmio (Emergency Fund, Investment Fund, Custom Goals)
- **Investment System**: Portfolio management, prodotti finanziari (ETF, azioni, obbligazioni, crypto)
- **Consultant Marketplace**: Matching investitori-consulenti con sistema di rating e commissioni
- **Gamification**: Sistema XP/livelli, badge, challenge con reward monetari
- **KYC/AML Compliance**: Verifica identità GDPR-compliant con document upload

### Business Model

- **Platform Fee**: 1% (min €1) su depositi, prelievi, investimenti, profitti
- **Marketplace**: 20% sulle commissioni dei consulenti

**Target**: Utenti retail 25-45 anni in Italia/EU interessati a risparmio guidato e investimenti.

---

## Database Schema

Il database implementa:
- **Table-Per-Hierarchy (TPH)** per User inheritance (Admin, Consultant, Investor)
- **13 indici unique** per constraint di integrità
- **15 indici compositi** per query ottimizzate
- **Foreign Keys con Restrict** per prevenire cancellazioni accidentali
- **Performance Score: 95/100**

### Entità Principali

| Tabella | Descrizione | Relazioni |
|---------|-------------|-----------|
| `Users` | Utenti (TPH: Admin, Consultant, Investor) | Base per tutte le entità user-centric |
| `Goals` | Obiettivi di risparmio | User ? Goals (1:N) |
| `Investments` | Investimenti in prodotti finanziari | User ? Investments (1:N) |
| `Portfolios` | Portafogli di investimento | User ? Portfolios (1:N) |
| `KycVerifications` | Verifiche KYC/AML | User ? KYC (1:1) |
| `PlatformFees` | Fee del 1% su transazioni | User ? Fees (1:N) |
| `Transactions` | Movimenti wallet | Account ? Transactions (1:N) |

Vedi [docs/technical/DATABASE_AUDIT.md](docs/technical/DATABASE_AUDIT.md) per dettagli completi.

---

## Autenticazione

**Provider**: Keycloak (self-hosted)  
**Protocollo**: OpenID Connect (OIDC) / OAuth 2.0  
**Token**: JWT con refresh token

### Ruoli Utente

| Ruolo | Descrizione | Permessi |
|-------|-------------|----------|
| `admin` | Amministratore sistema | Accesso completo, gestione utenti, report |
| `consultant` | Consulente finanziario | Gestione clienti, portfolio, commissioni |
| `investor` | Investitore/risparmiatore | Wallet, goals, investments, KYC |
| `kyc-verified` | Utente verificato | Accesso a funzionalità premium |

### Utenti Predefiniti (Development)

| Email | Password | Ruolo |
|-------|----------|-------|
| admin@goalgrow.com | Admin123! | admin, kyc-verified |
| consultant@goalgrow.com | Consultant123! | consultant, kyc-verified |
| investor@goalgrow.com | Investor123! | investor |

---

## Documentazione

La documentazione completa è disponibile nella cartella [`/docs`](docs/):

### Guida Setup
- [Setup Completo](docs/setup/COMPLETE_SETUP_GUIDE.md) - Installazione step-by-step
- [Getting Started](docs/GETTING_STARTED.md) - Guida rapida sviluppatori

### Architettura e Technical
- [Architecture Overview](docs/technical/ARCHITECTURE.md) - Architettura sistema
- [Database Audit](docs/technical/DATABASE_AUDIT.md) - Schema DB, FK, indici
- [Authentication](docs/technical/AUTHENTICATION.md) - Keycloak integration

### Business
- [Business Requirements](docs/BUSINESS_REQUIREMENTS.md) - Requisiti funzionali, KPI, revenue model
- [Roadmap](docs/ROADMAP.md) - Piano sviluppo MVP

### Navigazione
- [Indice Documentazione](docs/INDEX.md) - Hub centrale documentazione

---

## Sviluppo

### Build e Test

```bash
# Build soluzione
dotnet build

# Test (planned)
dotnet test

# Run API
cd GoalGrow.API
dotnet run
```

### Migration Database

```bash
cd GoalGrow.Migration

# Crea nuova migration
dotnet ef migrations add <NomeMigration> --project ../GoalGrow.Data --startup-project .

# Applica migration
dotnet ef database update --project ../GoalGrow.Data --startup-project .

# Rollback
dotnet ef database update <PreviousMigrationName> --project ../GoalGrow.Data --startup-project .
```

### Convenzioni Codice

- **C# 14.0** con nullable reference types
- **Async/await** per operazioni I/O
- **Dependency Injection** via ASP.NET Core DI
- **Configuration** via User Secrets (dev) / Azure Key Vault (prod)
- **Logging** via ILogger (Serilog planned)

---

## Performance

### Database Optimizations

- Query login by email: **< 5ms**
- User transactions (30 days): **< 15ms**
- Portfolio valuation: **< 20ms**
- Budget queries: **< 10ms**

### API Response Times (Target)

- GET endpoints: **< 100ms** (p95)
- POST endpoints: **< 200ms** (p95)
- Complex queries: **< 500ms** (p95)

---

## Compliance & Security

### GDPR
- Data residency EU
- Right to be forgotten
- Data retention policies (KYC: 5 anni)

### Security
- JWT token authentication
- Password hashing (Keycloak Argon2)
- HTTPS only (TLS 1.3)
- SQL injection prevention (parameterized queries)
- XSS/CSRF protection

### Financial Compliance
- **MIFID II**: Risk profiling obbligatorio
- **PSD2**: Open Banking integration (planned)
- **KYC/AML**: Document verification, PEP screening, sanctions list

---

## Roadmap

### Current Phase: Foundation ?
- [x] Database schema e migration
- [x] Keycloak authentication
- [x] API basic endpoints (login, health)
- [x] Documentazione tecnica

### Next Phase: Core API (Q1 2025)
- [ ] User management endpoints
- [ ] Goals CRUD
- [ ] Investments CRUD
- [ ] Transaction management
- [ ] KYC submission flow

### Future Phases
- [ ] Consultant marketplace
- [ ] Gamification system
- [ ] Blazor Web App
- [ ] .NET MAUI Mobile App
- [ ] Real-time notifications
- [ ] Advanced analytics

Vedi [docs/ROADMAP.md](docs/ROADMAP.md) per dettagli completi.

---

## Contribuire

Contributi sono benvenuti! Per favore:
1. Fork del repository
2. Crea feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add AmazingFeature'`)
4. Push al branch (`git push origin feature/AmazingFeature`)
5. Apri Pull Request

### Strategia di branching

Lo sviluppo è organizzato come segue:

- Il branch `develop` è il ramo principale di sviluppo: da `develop` si creano i branch per le feature (es. `feature/<nome>`).
- Le feature vengono sviluppate sui rispettivi branch e, quando pronte, vengono mergeate in `develop`.
- Periodicamente il contenuto di `develop` viene promosso su un branch `test` per le verifiche e la validazione.
- Dopo il ciclo di test, le modifiche verificate vengono unite nel branch `master` (ramo di produzione).

Seguire questa strategia per mantenere un flusso di lavoro ordinato e facilitare le review e il rilascio.

---

## License

Questo progetto è distribuito sotto licenza MIT. Vedi file [LICENSE](LICENSE) per dettagli.

---

## Contatti

**Edoardo Carollo**  
- GitHub: [@EdoardoCarollo99](https://github.com/EdoardoCarollo99)
- Email: edoardo.carollo@example.com
- Repository: [GoalGrow](https://github.com/EdoardoCarollo99/GoalGrow)

---

## Acknowledgments

Costruito utilizzando le migliori pratiche .NET:
- Clean Architecture (Robert C. Martin)
- Domain-Driven Design (Eric Evans)
- SOLID Principles
- Repository Pattern
- Dependency Injection

**Tecnologie Open Source:**
- [Keycloak](https://www.keycloak.org/) - Identity and Access Management
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - ORM
- [Scalar](https://github.com/scalar/scalar) - API Documentation

---

**Status Progetto**: ?? In Active Development  
**Ultima Revisione**: 2025-01-18  
**Versione**: 0.1.0-alpha

