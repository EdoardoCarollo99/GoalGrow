# ?? GoalGrow Documentation

Welcome to the GoalGrow documentation hub. This is your central reference for understanding and working with the platform.

---

## ?? Documentation Structure

### ?? Getting Started
- **[Getting Started Guide](GETTING_STARTED.md)** - Setup and first steps
- **[System Overview](SYSTEM_OVERVIEW.md)** - Architecture and components
- **[User Journey](USER_JOURNEY.md)** - End-to-end user experience

### ?? Technical Documentation
- **[Database Schema](technical/DATABASE.md)** - Tables, relationships, migrations
- **[Authentication](technical/AUTHENTICATION.md)** - Keycloak & JWT setup
- **[API Reference](technical/API_REFERENCE.md)** - REST API endpoints *(coming soon)*
- **[Domain Models](technical/DOMAIN_MODELS.md)** - Entity reference
- **[Value Objects](technical/VALUE_OBJECTS.md)** - Money, DateRange, Rating
- **[Services](technical/SERVICES.md)** - Business logic layer *(coming soon)*

### ?? Architecture
- **[Architecture Overview](technical/ARCHITECTURE.md)** - Clean Architecture layers
- **[Domain Modules](technical/MODULES.md)** - Financial, Investment, Gamification
- **[Design Patterns](technical/DESIGN_PATTERNS.md)** - DDD, Repository, CQRS

### ?? Diagrams
- **[System Overview](diagrams/01-SystemOverview.puml)** - High-level components
- **[User Management](diagrams/02-UserManagement.puml)** - Auth & users
- **[Financial Core](diagrams/03-FinancialCore.puml)** - Accounts, goals, budgets
- **[Investment System](diagrams/04-InvestmentSystem.puml)** - Portfolios & products
- **[Gamification](diagrams/05-Gamification.puml)** - XP, badges, challenges

### ?? Development
- **[Roadmap](ROADMAP.md)** - Feature timeline (11-month MVP)
- **[Contributing Guide](CONTRIBUTING.md)** - How to contribute
- **[Testing Strategy](technical/TESTING.md)** - Unit, integration, E2E
- **[Deployment](technical/DEPLOYMENT.md)** - Azure setup & CI/CD

### ?? Business
- **[Business Requirements](BUSINESS_REQUIREMENTS.md)** - Goals, KPIs, revenue model
- **[Compliance](technical/COMPLIANCE.md)** - GDPR, KYC/AML, MIFID II
- **[Security](technical/SECURITY.md)** - Best practices & audit

---

## ?? Quick Reference

### Common Tasks

| Task | Documentation |
|------|---------------|
| Setup development environment | [Getting Started](GETTING_STARTED.md) |
| Run database migrations | [Database Guide](technical/DATABASE.md#migrations) |
| Configure Keycloak | [Authentication](technical/AUTHENTICATION.md#keycloak-setup) |
| Add new entity | [Domain Models](technical/DOMAIN_MODELS.md#creating-entities) |
| View UML diagrams | [Diagrams](diagrams/) |
| Check project status | [CHANGELOG.md](../CHANGELOG.md) |

### Technology Stack

| Component | Documentation |
|-----------|---------------|
| .NET 10 | [Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10) |
| EF Core 10 | [Database Guide](technical/DATABASE.md) |
| Blazor | *(coming soon)* |
| Keycloak | [Authentication](technical/AUTHENTICATION.md) |
| SQL Server | [Database Guide](technical/DATABASE.md) |

---

## ?? External Resources

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) by Uncle Bob
- [Domain-Driven Design](https://martinfowler.com/bliki/DomainDrivenDesign.html) by Martin Fowler
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [PlantUML Guide](https://plantuml.com/)

---

## ?? Support

### Getting Help

1. **Check Documentation** - Start here in `/docs`
2. **Read CHANGELOG** - See [CHANGELOG.md](../CHANGELOG.md) for recent changes
3. **View Examples** - Check `DatabaseSeeder.cs` for sample data
4. **Open Issue** - [GitHub Issues](https://github.com/EdoardoCarollo99/GoalGrow/issues)

### Reporting Issues

When reporting bugs, include:
- Steps to reproduce
- Expected vs actual behavior
- Environment (.NET version, OS, database)
- Relevant logs

---

## ?? Documentation Standards

### File Naming
- Use `SCREAMING_SNAKE_CASE.md` for root-level docs
- Use `PascalCase.md` for nested technical docs
- Use `kebab-case.puml` for diagrams

### Structure
- Use emoji for better readability (?? ?? ??)
- Include table of contents for long docs
- Link between related documents
- Keep code examples up-to-date

---

## ??? Document Map

```
docs/
??? INDEX.md (you are here)
??? GETTING_STARTED.md
??? SYSTEM_OVERVIEW.md
??? USER_JOURNEY.md
??? BUSINESS_REQUIREMENTS.md
??? ROADMAP.md
??? CONTRIBUTING.md
??? diagrams/
?   ??? 01-SystemOverview.puml
?   ??? 02-UserManagement.puml
?   ??? 03-FinancialCore.puml
?   ??? 04-InvestmentSystem.puml
?   ??? 05-Gamification.puml
??? technical/
    ??? ARCHITECTURE.md
    ??? DATABASE.md
    ??? AUTHENTICATION.md
    ??? DOMAIN_MODELS.md
    ??? VALUE_OBJECTS.md
    ??? MODULES.md
    ??? DESIGN_PATTERNS.md
    ??? API_REFERENCE.md
    ??? SERVICES.md
    ??? TESTING.md
    ??? DEPLOYMENT.md
    ??? COMPLIANCE.md
    ??? SECURITY.md
```

---

## ?? Keeping Documentation Updated

- Update docs when changing code
- Link PRs to related documentation updates
- Review docs during code reviews
- Run examples to verify accuracy

---

**Last Updated:** 2025-01-18  
**Version:** 1.0  
**Maintained by:** Edoardo Carollo
