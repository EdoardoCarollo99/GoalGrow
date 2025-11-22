# GoalGrow - Indice Documentazione

Benvenuto nella documentazione completa di **GoalGrow**. Questa pagina è il punto di partenza per navigare tra i vari documenti tecnici, business e architetturali del progetto.

---

## ?? Navigazione Rapida

### Per Iniziare
- **[Setup Completo](setup/COMPLETE_SETUP_GUIDE.md)** - Guida passo-passo per configurare ambiente di sviluppo
- **[Getting Started](GETTING_STARTED.md)** - Quick start per sviluppatori
- **[Start Here](setup/START_HERE.md)** - Prima installazione

### Architettura e Design
- **[Architecture Overview](technical/ARCHITECTURE.md)** - Architettura sistema e pattern
- **[Database Schema](technical/DATABASE_AUDIT.md)** - Schema DB, foreign keys, indici
- **[Authentication](technical/AUTHENTICATION.md)** - Integrazione Keycloak OIDC

### Business e Requisiti
- **[Business Requirements](BUSINESS_REQUIREMENTS.md)** - Requisiti funzionali, KPI, revenue model
- **[Roadmap](ROADMAP.md)** - Piano sviluppo e milestone
- **[Task List](TASK_LIST.md)** - Checklist implementazione

---

## ??? Struttura Documentazione

```
docs/
??? INDEX.md                          # Questo file
??? GETTING_STARTED.md                # Quick start
??? BUSINESS_REQUIREMENTS.md          # Requisiti business
??? ROADMAP.md                        # Piano sviluppo
??? TASK_LIST.md                      # Checklist task
??? IMPLEMENTATION_CHECKLIST.md       # Checklist implementazione
?
??? setup/                            # Guide installazione
?   ??? COMPLETE_SETUP_GUIDE.md       # Setup completo step-by-step
?   ??? SETUP_GUIDE.md                # Setup rapido
?   ??? START_HERE.md                 # Prima installazione
?
??? technical/                        # Documentazione tecnica
?   ??? ARCHITECTURE.md               # Architettura sistema
?   ??? AUTHENTICATION.md             # Autenticazione Keycloak
?   ??? DATABASE_AUDIT.md             # Audit database
?   ??? DATABASE_OPTIMIZATION_SUMMARY.md  # Ottimizzazioni DB
?
??? architecture/                     # Design architetturale
?   ??? CLEAN_ARCHITECTURE.md         # Clean Architecture
?   ??? DOMAIN_DRIVEN_DESIGN.md       # DDD pattern
?   ??? LAYERS.md                     # Divisione layer
?
??? business/                         # Documentazione business
    ??? USER_JOURNEY.md               # Customer journey
    ??? REVENUE_MODEL.md              # Modello revenue
    ??? COMPLIANCE.md                 # GDPR, KYC, MIFID II
```

---

## ?? Documenti per Categoria

### Setup e Installazione

| Documento | Descrizione | Target |
|-----------|-------------|--------|
| [Complete Setup Guide](setup/COMPLETE_SETUP_GUIDE.md) | Guida completa installazione ambiente | Nuovi sviluppatori |
| [Setup Guide](setup/SETUP_GUIDE.md) | Setup rapido | Sviluppatori esperti |
| [Start Here](setup/START_HERE.md) | Prima installazione | Tutti |

### Tecnica

| Documento | Descrizione | Target |
|-----------|-------------|--------|
| [Architecture](technical/ARCHITECTURE.md) | Panoramica architettura | Architetti, dev senior |
| [Database Audit](technical/DATABASE_AUDIT.md) | Schema DB, FK, indici, performance | DBA, backend dev |
| [Authentication](technical/AUTHENTICATION.md) | Keycloak OIDC integration | Backend dev, security |
| [Database Optimization](technical/DATABASE_OPTIMIZATION_SUMMARY.md) | Riepilogo ottimizzazioni DB | DBA |

### Business e Prodotto

| Documento | Descrizione | Target |
|-----------|-------------|--------|
| [Business Requirements](BUSINESS_REQUIREMENTS.md) | Requisiti funzionali completi | PM, stakeholder |
| [Roadmap](ROADMAP.md) | Piano sviluppo e milestone | PM, dev team |
| [Implementation Checklist](IMPLEMENTATION_CHECKLIST.md) | Checklist task implementazione | Dev team |

---

## ?? Percorsi di Lettura Consigliati

### Per Nuovi Sviluppatori

1. **[README principale](../README.md)** - Panoramica progetto
2. **[Getting Started](GETTING_STARTED.md)** - Setup ambiente
3. **[Complete Setup Guide](setup/COMPLETE_SETUP_GUIDE.md)** - Installazione dettagliata
4. **[Architecture](technical/ARCHITECTURE.md)** - Comprensione architettura
5. **[Database Audit](technical/DATABASE_AUDIT.md)** - Schema database

### Per Product Manager / Stakeholder

1. **[README principale](../README.md)** - Overview tecnico
2. **[Business Requirements](BUSINESS_REQUIREMENTS.md)** - Requisiti completi
3. **[Roadmap](ROADMAP.md)** - Piano sviluppo
4. **User Journey** *(coming soon)* - Customer experience

### Per DevOps / SRE

1. **[Setup Guide](setup/SETUP_GUIDE.md)** - Ambiente deployment
2. **[Database Audit](technical/DATABASE_AUDIT.md)** - Performance DB
3. **[Authentication](technical/AUTHENTICATION.md)** - Security setup
4. **Deployment Guide** *(coming soon)* - CI/CD pipeline

---

## ?? Aggiornamenti Documentazione

| Data | Versione | Modifiche |
|------|----------|-----------|
| 2025-01-18 | 0.1.0 | Creazione struttura documentazione iniziale |
| 2025-01-18 | 0.1.1 | Riorganizzazione file MD in cartelle tematiche |

---

## ?? Convenzioni Documentazione

### Naming
- `SCREAMING_SNAKE_CASE.md` per documenti root
- `PascalCase.md` per documenti nested
- `kebab-case.puml` per diagrammi PlantUML

### Struttura Documenti
- Usare emoji per migliorare leggibilità (?? ?? ???)
- Includere Table of Contents per documenti >500 righe
- Link tra documenti correlati
- Mantenere esempi codice aggiornati

### Metadata
Ogni documento dovrebbe includere:
- **Autore**: Nome autore principale
- **Data**: Data ultima revisione
- **Versione**: Versione documento
- **Status**: Draft / Review / Approved

---

## ?? Supporto

### Documentazione Mancante?

Se non trovi la documentazione che cerchi:
1. Controlla [GitHub Issues](https://github.com/EdoardoCarollo99/GoalGrow/issues)
2. Cerca nell'indice sopra
3. Apri una Issue con label `documentation`

### Contribuire alla Documentazione

1. Fork repository
2. Crea branch `docs/nome-documento`
3. Aggiungi/modifica documento
4. Aggiorna questo INDEX.md
5. Apri Pull Request

---

**Ultima Revisione**: 2025-01-18  
**Versione Documentazione**: 1.0.0  
**Maintainer**: Edoardo Carollo
