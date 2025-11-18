#  Changelog

All notable changes to GoalGrow will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Planned
- API project creation (GoalGrow.Api)
- Blazor Web App (GoalGrow.Web)
- MAUI Mobile App
- Keycloak integration
- KYC verification workflow
- Investment marketplace

---

## [0.2.0] - 2025-01-18

### Added - Architecture Refactoring

#### New Domain Concepts
- **Value Objects** for cleaner domain logic
  - `Money` - Currency-aware decimal values with operations
  - `DateRange` - Time period validation and utilities
  - `Rating` - User ratings with review text
  - `ContactInfo` - Structured contact information

- **Base Classes** to reduce code duplication
  - `AuditableEntity` - CreatedAt, UpdatedAt tracking
  - `FullAuditableEntity` - Adds soft delete support

- **New Entities** for compliance and monetization
  - `KycVerification` - KYC/AML compliance tracking
  - `PlatformFee` - Revenue tracking (1% fees)
  
- **Enhanced Entities**
  - `Goal` - Added `GoalType` enum, system goals (Emergency, Investment)
  - `Goal` - Added `IsSystemGoal`, `UnlockThreshold`, `IsWithdrawalLocked`
  - `User` - Added `KeycloakSubjectId` for OIDC integration

#### New Enums
- `GoalType` - Emergency, Investment, Custom, Travel, etc.
- `KycStatus` - KYC verification workflow states
- `PlatformFeeType` - Fee categories
- `FeeStatus` - Fee collection status

#### Module-Specific DbContexts
- `FinancialDbContext` - Account, Transaction, Budget, Goal
- `InvestmentDbContext` - Portfolio, Investment, RiskProfile
- `GamificationDbContext` - Badge, Challenge, UserLevel

#### Documentation
- Reorganized documentation into `/docs` folder
- Created central documentation hub (`docs/INDEX.md`)
- Moved diagrams to `docs/diagrams/`
- Moved technical docs to `docs/technical/`
- Created comprehensive business requirements doc
- Added authentication architecture guide
- Created PlantUML diagrams for all modules:
  - System Overview
  - User Management
  - Financial Core
  - Investment System
  - Gamification

### Changed
- Refactored `GoalGrowDbContext` with region-based organization
- Updated `Goal.cs` to support system goals and investment unlocking
- Reorganized EF Core configurations into module folders:
  - `Configurations/User/`
  - `Configurations/Financial/`
  - `Configurations/Investment/`
  - `Configurations/Gamification/`

### Technical Improvements
- Applied Domain-Driven Design (DDD) principles
- Implemented Clean Architecture layers
- Introduced Value Objects pattern
- Added comprehensive XML documentation
- Created migration automation script (`Migrate-Configurations.ps1`)

---

## [0.1.0] - 2025-01-16

### Added - Initial Database Schema

#### Core Entities
- **User Management**
  - `User` (abstract base class)
  - `InversotorUser` - Investor user with virtual wallet
  - `ConsultantUser` - Financial consultant with commission tracking
  - `UserConsultantRelationship` - 1:1 investor-consultant pairing

- **Financial Core**
  - `Account` - Bank accounts
  - `Transaction` - Financial transactions
  - `RecurringTransaction` - Automated recurring payments
  - `Payee` - Transaction recipients
  - `Budget` - Category-based budget tracking
  - `Goal` - Savings goals with auto-save

- **Investment System**
  - `Portfolio` - Investment portfolio container
  - `Investment` - Individual investment positions
  - `InvestmentProduct` - Available financial products
  - `CompanyAccount` - Central company wallet for user funds
  - `FundMovement` - Deposits/withdrawals to company account
  - `RiskProfile` - MIFID II compliant risk assessment
  - `CommissionTransaction` - Consultant earning tracking
  - `PortfolioSnapshot` - Performance history snapshots

- **Gamification**
  - `Badge` - Available achievements
  - `UserBadge` - Earned badges
  - `Challenge` - Financial challenges
  - `UserChallenge` - User challenge progress
  - `UserLevel` - XP and level system

- **Notifications**
  - `Notification` - User alert system

#### Entity Framework
- Configured all entity relationships
- Implemented Table-Per-Hierarchy (TPH) for User inheritance
- Created comprehensive EF Core configurations
- Added indexes for performance optimization
- Configured cascade delete behaviors

#### Database Infrastructure
- `GoalGrowDbContext` - Main database context
- `GoalGrowDbContextFactory` - Design-time factory for migrations
- `DatabaseSeeder` - Test data generation
- User Secrets configuration for connection strings

#### Migration & Seeding
- Initial migration: `InitialCreate`
- Database seeder with sample data:
  - 3 test users (Admin, Investor, Consultant)
  - Default system goals (Emergency, Investment)
  - Sample transactions and budgets
  - Investment products and portfolios
  - Gamification data (badges, challenges)

### Technical Stack
- .NET 10
- Entity Framework Core 10
- SQL Server
- User Secrets for configuration

### Documentation
- Created `README.md` with project overview
- Added `EF_CORE_CONSTRUCTOR_FIX.md` for constructor patterns
- Created `QUICK_START.md` for setup instructions
- Initial `ARCHITECTURE.md` documentation

---

## Version History

### Current Phase
**Phase:** Foundation & Database Setup  
**Status:** In Development  
**Next Milestone:** API Project Creation

### Upcoming Versions

#### [0.3.0] - Planned (Q2 2025)
- API project with JWT authentication
- User registration and KYC endpoints
- Goal management endpoints
- Transaction endpoints
- Keycloak integration

#### [0.4.0] - Planned (Q2 2025)
- Blazor Web App
- User dashboard
- Goal tracking UI
- Transaction history
- KYC upload interface

#### [0.5.0] - Planned (Q3 2025)
- Investment system implementation
- Consultant marketplace
- Portfolio management UI
- Risk profiling questionnaire

#### [0.6.0] - Planned (Q3 2025)
- Gamification features
- XP and level tracking
- Badge unlocking
- Challenge system with rewards

#### [0.7.0] - Planned (Q4 2025)
- MAUI mobile app
- Push notifications
- Biometric authentication

#### [1.0.0] - MVP Target (Q4 2025)
- Full feature set
- Production-ready
- Security audit completed
- Performance optimized
- Documentation complete

---

## Migration Notes

### Database Migrations

#### How to Create a Migration
```bash
cd GoalGrow.Migration
dotnet ef migrations add MigrationName --project ../GoalGrow.Data --startup-project .
```

#### How to Apply Migrations
```bash
dotnet ef database update --project ../GoalGrow.Data --startup-project .
```

#### How to Rollback
```bash
dotnet ef database update PreviousMigrationName --project ../GoalGrow.Data --startup-project .
```

#### Migration History
1. `InitialCreate` (2025-01-16) - Initial schema
2. *(Future migrations will be listed here)*

---

## Breaking Changes

### [0.2.0]
- **Configuration Organization:** EF configurations moved to module-specific folders. If you have custom configurations, update namespaces.
- **Goal Entity:** Added new required fields (`Type`, `IsSystemGoal`). Run migrations before deploying.

---

## Contributors

- **Edoardo Carollo** - Initial work and architecture

---

## Acknowledgments

- Inspired by modern fintech apps (Revolut, N26, Robinhood)
- Architecture patterns from Eric Evans (DDD) and Uncle Bob (Clean Architecture)
- Community feedback and suggestions

---

** For detailed technical changes, see commit history on GitHub.**
