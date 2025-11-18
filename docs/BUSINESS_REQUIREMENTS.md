# ?? GoalGrow - Business Requirements Document

## Executive Summary

**GoalGrow** è una piattaforma fintech B2C che combina:
- Gestione finanziaria personale con obiettivi di risparmio
- Marketplace di consulenti finanziari
- Sistema di investimenti guidato
- Gamification per engagement

**Target Market:** Utenti retail italiani/europei che vogliono risparmiare e investire in modo guidato.

**Revenue Model:** Fee dell'1% (min 1€) su depositi, prelievi, investimenti e profitti.

---

## ?? User Personas

### 1. **Investor User** (Utente Risparmiatore)
- Età: 25-45 anni
- Obiettivo: Risparmiare e iniziare a investire
- Pain Point: Non sa da dove iniziare con gli investimenti
- Valore offerto: Percorso guidato dal risparmio all'investimento

### 2. **Consultant User** (Consulente Finanziario)
- Professionista con licenza
- Obiettivo: Trovare nuovi clienti
- Pain Point: Difficile acquisire clienti retail
- Valore offerto: Marketplace di clienti pre-qualificati (KYC fatto)

---

## ?? User Journey

### Phase 1: Onboarding & KYC
```
1. Registrazione (Email/Social + Keycloak OIDC)
   ?
2. KYC Verification (carica documenti ID)
   ?
3. Creazione account (2 obiettivi default creati automaticamente)
   - ?? Fondo Emergenza (svincolabile)
   - ?? Fondo Investimenti (bloccato fino a soglia)
```

### Phase 2: Risparmio
```
4. Collegamento conto bancario (future: Open Banking API)
   ?
5. Deposito fondi in virtual wallet (fee 1%, min 1€)
   ?
6. Allocazione fondi su obiettivi:
   - Automatico: 50% Emergenza, 50% Investimenti
   - Manuale: crea obiettivi personalizzati (Vacanze, Auto, ecc.)
   ?
7. Auto-save ricorrente (settimanale/mensile)
```

### Phase 3: Sblocco Investimenti
```
8. Fondo Investimenti raggiunge soglia (es. 5000€)
   ?
9. Notifica: "Sblocca marketplace consulenti!"
   ?
10. Compilazione RiskProfile (MIFID II compliant)
```

### Phase 4: Selezione Consulente
```
11. Browse marketplace consulenti filtrato per:
    - Specializzazione (azioni, crypto, immobiliare)
    - Rating utenti
    - Commissioni
    - Anni esperienza
    ?
12. Prenotazione call conoscitiva (calendly-style)
    ?
13. Attivazione rapporto consulente-investitore
```

### Phase 5: Investimento
```
14. Consulente propone portfolio personalizzato
    ?
15. Utente approva e trasferisce fondi da "Fondo Investimenti"
    ?
16. Acquisto prodotti finanziari (azioni, ETF, fondi)
    ?
17. Tracking performance in real-time
```

### Phase 6: Gamification
```
18. Guadagna XP e livelli:
    - Primo deposito (+100 XP)
    - Primo obiettivo raggiunto (+200 XP)
    - Primo investimento (+500 XP)
    ?
19. Unlock badge:
    - "First Saver" ??
    - "Investment Starter" ??
    - "Portfolio Builder" ??
    ?
20. Completa challenge:
    - "Risparmia 1000€ in 30 giorni" (reward: 50€ bonus)
```

---

## ?? Revenue Model

### Platform Fees (GoalGrow Revenue)

| Transaction Type | Fee | Minimum | Example |
|------------------|-----|---------|---------|
| Deposit | 1% | 1€ | 300€ deposit = 3€ fee |
| Withdrawal | 1% | 1€ | 200€ withdrawal = 2€ fee |
| Investment | 1% | 1€ | 5000€ investment = 50€ fee |
| Investment Profit | 1% | 1€ | 1000€ profit = 10€ fee |
| Goal Transfer | 1% | 1€ | Transfer 500€ = 5€ fee |

**Estimated Revenue:**
- User con 10K€ risparmiati/anno = ~150€ fee/anno
- 1,000 utenti attivi = 150K€/anno
- 10,000 utenti attivi = 1.5M€/anno

### Consultant Commissions (Consultant Revenue)

| Service | Commission | Example |
|---------|------------|---------|
| Investment Management | 0.5-2% AUM/anno | 100K€ portfolio = 500-2000€/anno |
| One-time Advisory | Flat fee | 500-2000€ |
| Performance Fee | 10-20% profits | 10K€ profit = 1-2K€ |

**GoalGrow's Cut:** 20% delle commissioni consulente (es. su 1000€, prendi 200€)

---

## ??? Technical Architecture

### Platform Type
- **Primary**: Blazor Web App (Server-Side)
- **Mobile**: MAUI App (iOS/Android)
- **Backend**: ASP.NET Core Web API (.NET 10)

### Authentication
- **Provider**: Keycloak (self-hosted) - See [AUTHENTICATION_ARCHITECTURE.md](AUTHENTICATION_ARCHITECTURE.md)
- **Protocol**: OpenID Connect (OIDC)
- **Tokens**: JWT with refresh tokens

### Database
- **Main**: SQL Server (Azure SQL / on-premise)
- **Caching**: Redis (future)
- **Search**: Elasticsearch (future, per marketplace)

### Infrastructure
- **Hosting**: Azure App Service / Kubernetes
- **Storage**: Azure Blob Storage (documenti KYC)
- **CDN**: Azure CDN (immagini, static assets)

### Third-Party Integrations

| Service | Provider | Purpose |
|---------|----------|---------|
| KYC/AML | Onfido / Jumio | Document verification |
| Open Banking | TrueLayer / Plaid | Bank account linking |
| Payment Gateway | Stripe / Adyen | Deposits/Withdrawals |
| Market Data | Alpha Vantage / Yahoo Finance | Stock prices |
| Notifications | SendGrid / Twilio | Email/SMS |
| Video Calls | Twilio Video / Daily.co | Consulente-cliente calls |

---

## ?? Data Model Summary

### Core Entities

```
User (abstract)
??? InversotorUser
?   ??? VirtualWalletBalance
?   ??? KycVerification (1:1)
?   ??? RiskProfile (1:1)
?   ??? Goals (1:N)
?   ??? Portfolios (1:N)
?   ??? UserConsultantRelationship (1:1)
?
??? ConsultantUser
    ??? LicenseNumber
    ??? CommissionRate
    ??? Clients (1:N UserConsultantRelationship)
    ??? Commissions (1:N CommissionTransaction)

Goal (con 2 default system goals)
??? Emergency (IsSystemGoal=true, Unlockable)
??? Investment (IsSystemGoal=true, Locked until threshold)
??? Custom Goals (Vacanze, Auto, ecc.)

FundMovement (Virtual Wallet)
??? Deposit (User Account ? Company Account)
??? Withdrawal (Company Account ? User Account)
??? Investment (Company Account ? Portfolio)

PlatformFee
??? On Deposit (1%, min 1€)
??? On Withdrawal (1%, min 1€)
??? On Investment/Profit (1%, min 1€)
```

---

## ?? Compliance & Security

### KYC/AML Requirements

**For All Users:**
- ? Email verification
- ? Phone verification (OTP)
- ? Document upload (ID/Passport)
- ? Selfie verification
- ? Proof of address
- ? PEP screening
- ? Sanctions list check

**For Consultants (Additional):**
- ? Professional license verification
- ? CONSOB registration (Italy)
- ? Background check

### Data Protection
- **GDPR Compliant**: EU data residency, right to be forgotten
- **Encryption**: AES-256 at rest, TLS 1.3 in transit
- **Data Retention**: KYC docs 5 years (legal requirement)

### Financial Regulation
- **MIFID II**: Risk profiling obbligatorio
- **PSD2**: Open Banking compliance
- **Anti-Money Laundering**: Transaction monitoring

---

## ?? Gamification Mechanics

### Level System
```
Level 1: Beginner (0-100 XP)
Level 2: Saver (100-300 XP)
Level 3: Investor (300-700 XP)
Level 4: Portfolio Builder (700-1500 XP)
Level 5: Wealth Master (1500+ XP)
```

### XP Earning Actions
| Action | XP | Trigger |
|--------|----|---------| 
| Complete KYC | 50 | One-time |
| First Deposit | 100 | One-time |
| Reach Emergency Goal | 200 | Per goal |
| First Investment | 500 | One-time |
| Monthly Savings Streak | 50 | Monthly |
| Refer a Friend | 100 | Per referral |

### Badge Categories
- **Achievement**: "First Million", "Debt-Free"
- **Milestone**: "1 Year Saver", "10 Investments"
- **Special**: "Early Adopter", "Beta Tester"

### Challenges (with Money Rewards)
- "Save 1000€ in 30 days" ? 50€ reward
- "Complete 10 recurring deposits" ? 20€ reward
- "Reach investment threshold" ? 100€ reward

---

## ?? Success Metrics (KPIs)

### User Acquisition
- Monthly Active Users (MAU)
- New Registrations
- KYC Completion Rate
- Cost Per Acquisition (CPA)

### Engagement
- Average Session Duration
- Goals Created per User
- Auto-Save Activation Rate
- App Opens per Week

### Revenue
- Total Deposits (TVL - Total Value Locked)
- Fee Revenue
- Average Revenue Per User (ARPU)
- Consultant Marketplace Take Rate

### Investment
- Investment Threshold Unlock Rate
- Consultant Matching Success Rate
- Average Portfolio Size
- Assets Under Management (AUM)

### Gamification
- Badge Unlock Rate
- Challenge Completion Rate
- XP Growth Rate
- Referral Rate

---

## ?? MVP (Minimum Viable Product)

### Phase 1: Core Savings (3 months)
- ? User registration + KYC
- ? Virtual wallet deposit/withdrawal
- ? Goal creation (Emergency + Investment + Custom)
- ? Auto-save recurring deposits
- ? Basic transaction tracking

### Phase 2: Consultant Marketplace (2 months)
- ? Consultant registration + verification
- ? RiskProfile questionnaire
- ? Marketplace listing and search
- ? Consultant-investor matching
- ? In-app messaging/video calls

### Phase 3: Investment System (3 months)
- ? Portfolio creation
- ? Investment product catalog
- ? Buy/Sell investments
- ? Performance tracking
- ? Commission tracking

### Phase 4: Gamification (1 month)
- ? XP and levels
- ? Badge system
- ? Challenges with rewards
- ? Leaderboard

### Phase 5: Mobile App (2 months)
- ? MAUI cross-platform app
- ? Push notifications
- ? Biometric login

**Total MVP Timeline: 11 months**

---

## ?? Future Roadmap

### Year 1
- Launch in Italy
- 1,000 active users
- 50 verified consultants
- €5M TVL (Total Value Locked)

### Year 2
- Expand to Spain, France, Germany
- 10,000 active users
- 500 consultants
- €50M TVL
- Add crypto investments

### Year 3
- Pan-European expansion
- 100,000 users
- B2B offering (white-label for banks)
- €500M TVL
- IPO considerations

---

## ?? Competitive Advantages

1. **Gamification**: No competitor has true gamification with monetary rewards
2. **Guided Path**: Clear journey from saving to investing (not overwhelming)
3. **Consultant Marketplace**: Two-sided platform creates network effects
4. **System Goals**: Auto-created Emergency + Investment goals (behavioral nudge)
5. **EU-First**: GDPR compliant from day 1, not an afterthought

---

## ?? Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| Regulatory changes | High | Legal team + compliance monitoring |
| Market volatility | Medium | Clear risk disclosures, conservative defaults |
| Consultant fraud | High | Strict KYC, license verification, insurance |
| User churn | Medium | Gamification, habit formation, auto-save |
| Security breach | Critical | Penetration testing, bug bounty, insurance |
| Cash flow (virtual wallet) | High | Segregated accounts, regular audits |

---

## ?? References

- [AUTHENTICATION_ARCHITECTURE.md](AUTHENTICATION_ARCHITECTURE.md) - Auth strategy
- [ARCHITECTURE_V2.md](ARCHITECTURE_V2.md) - Technical architecture
- [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) - Refactoring guide

---

**Document Version:** 1.0  
**Last Updated:** 2025-01-18  
**Owner:** Edoardo Carollo
