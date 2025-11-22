# Revenue Model - GoalGrow

## Modello di Business

GoalGrow genera revenue attraverso **2 flussi principali**:
1. **Platform Fees** (1% su transazioni)
2. **Marketplace Commission** (20% su commissioni consulenti)

---

## 1. Platform Fees

### Struttura Fee

| Operazione | Fee | Minimo | Applicazione |
|------------|-----|--------|--------------|
| **Deposit** | 1% | €1 | Su deposito wallet virtuale |
| **Withdrawal** | 1% | €1 | Su prelievo fondi |
| **Investment** | 1% | €1 | Su acquisto prodotto finanziario |
| **Profit Realization** | 1% | €1 | Su vendita investment con guadagno |

### Esempio Calcolo

```
Utente deposita €1,000 nel wallet virtuale:
- Amount deposited: €1,000
- Platform fee (1%): €10
- Net deposit: €990
- Fee collected: €10 ? GoalGrow revenue
```

### Implementazione Database

```csharp
public class PlatformFee
{
    public Guid Id { get; set; }
    public string FeeNumber { get; set; }
    public Guid UserId { get; set; }
    public PlatformFeeType Type { get; set; } // Deposit, Withdrawal, Investment, Profit
    
    // Calcolo fee
    public decimal BaseAmount { get; set; }        // €1,000
    public decimal FeePercentage { get; set; }     // 1.00%
    public decimal MinimumFee { get; set; }        // €1
    public decimal CalculatedFee { get; set; }     // Max(€1, €1,000 * 1%) = €10
    
    // Relazioni
    public Guid? RelatedTransactionId { get; set; }
    public Guid? RelatedInvestmentId { get; set; }
    
    // Status
    public FeeStatus Status { get; set; }          // Pending, Collected, Refunded
    public DateTime? CollectedAt { get; set; }
}
```

---

## 2. Marketplace Commission

### Struttura Commission

**Consulente** riceve commissione da investitore ? **GoalGrow** trattiene 20%

| Scenario | Commissione Consulente | GoalGrow Cut (20%) | Net Consulente |
|----------|------------------------|---------------------|-----------------|
| Portfolio €10,000 (1.5% annuo) | €150/anno | €30 | €120 |
| Investment €5,000 (2% fee) | €100 | €20 | €80 |

### Esempio Flusso

```
1. Investitore investe €10,000 in portfolio
2. Consulente ha commissione 1.5% annua = €150
3. GoalGrow calcola:
   - Total commission: €150
   - Platform cut (20%): €30 ? GoalGrow revenue
   - Net to consultant: €120
4. Pagamento:
   - Consulente riceve €120
   - GoalGrow trattiene €30
```

### Implementazione Database

```csharp
public class CommissionTransaction
{
    public Guid Id { get; set; }
    public Guid ConsultantId { get; set; }
    public Guid InvestorId { get; set; }
    public Guid? InvestmentId { get; set; }
    
    // Commission breakdown
    public decimal BaseAmount { get; set; }           // €10,000 (investment)
    public decimal CommissionRate { get; set; }       // 1.5%
    public decimal TotalCommission { get; set; }      // €150
    public decimal PlatformCut { get; set; }          // €30 (20%)
    public decimal NetToConsultant { get; set; }      // €120
    
    // Status
    public CommissionStatus Status { get; set; }      // Pending, Paid
    public DateTime? PaidAt { get; set; }
}
```

---

## Revenue Projections

### Scenario Base (Year 1)

**Assunzioni**:
- 1,000 utenti attivi
- €500 deposito medio mensile
- 30% utenti investono (300 investitori)
- €2,000 investimento medio
- 50 consulenti attivi
- 20% investitori usano consulente (60 match)

### Revenue Breakdown

#### Platform Fees (Deposits)
```
Monthly:
- 1,000 users × €500 deposit × 1% fee = €5,000
- Minimum fee impact: ~10% ? €5,500

Annual: €5,500 × 12 = €66,000
```

#### Platform Fees (Investments)
```
One-time (Year 1):
- 300 investors × €2,000 × 1% = €6,000

Recurring (withdrawals, profits): €12,000/year
```

#### Marketplace Commissions
```
Annual:
- 60 matched investors × €5,000 avg portfolio × 1.5% consultant rate = €4,500
- GoalGrow cut (20%): €900/year
```

### Total Year 1 Revenue

| Source | Amount |
|--------|--------|
| Platform Fees (Deposits) | €66,000 |
| Platform Fees (Investments) | €18,000 |
| Marketplace Commissions | €900 |
| **TOTAL** | **€84,900** |

---

## Revenue Growth Drivers

### 1. User Growth
- **Target**: 10,000 users entro Year 3
- **Acquisition**: Marketing digitale, referral program
- **Retention**: Gamification, rewards

### 2. Average Deposit Increase
- **Target**: €500 ? €1,000 entro Year 2
- **Strategy**: 
  - Challenge con reward
  - Auto-savings features
  - Salary integration

### 3. Investment Conversion
- **Target**: 30% ? 50% entro Year 2
- **Strategy**:
  - KYC frictionless
  - Educational content
  - Advisor matching

### 4. Consultant Network
- **Target**: 50 ? 200 consulenti entro Year 3
- **Strategy**:
  - Partnership OCF (Organismo Consulenti Finanziari)
  - Referral program consulenti
  - Tools analytics per consulenti

---

## Cost Structure

### Fixed Costs

| Voce | Costo Mensile | Costo Annuale |
|------|---------------|---------------|
| Infrastructure (Azure) | €500 | €6,000 |
| Keycloak hosting | €50 | €600 |
| Storage (KYC docs) | €100 | €1,200 |
| SSL, Domain | €20 | €240 |
| **TOTAL** | **€670** | **€8,040** |

### Variable Costs

| Voce | Costo per Transazione | Stima Annuale |
|------|-----------------------|---------------|
| Payment processing | €0.20 + 0.5% | €15,000 |
| SMS notifications | €0.05 | €2,000 |
| Email sending | €0.001 | €500 |
| **TOTAL** | - | **€17,500** |

### Total Costs Year 1

```
Fixed: €8,040
Variable: €17,500
TOTAL: €25,540
```

---

## Profitability Analysis

### Year 1

```
Revenue:    €84,900
Costs:      €25,540
Profit:     €59,360
Margin:     70%
```

### Break-Even Point

```
Fixed costs: €8,040/year
Revenue per user/month: €5.50 (deposit fee)

Break-even users: €8,040 / (€5.50 × 12) = 122 users

Raggiunto con < 150 utenti
```

---

## Pricing Alternatives (Future)

### Freemium Model
- **Free Tier**: Max 3 goals, no investment
- **Premium**: €9.99/month ? Unlimited goals, investment, advisor access
- **Pro**: €29.99/month ? Advanced analytics, priority support

### Subscription + Reduced Fees
- **€4.99/month**: Fee ridotta a 0.5% (invece di 1%)
- **Target**: High-volume traders

---

## Compliance & Transparency

### Fee Disclosure
Tutti i fee sono:
- ? **Comunicati prima** della transazione
- ? **Visualizzati** in transaction detail
- ? **Riportati** in monthly statement
- ? **Conformi** a MIFID II

### Example UI
```
Deposit: €1,000
Platform fee (1%): €10
Net deposited: €990

[Confirm Deposit]
```

---

## KPI Monitoring

### Key Metrics

| KPI | Definizione | Target Year 1 |
|-----|-------------|---------------|
| **ARPU** | Average Revenue Per User | €85 |
| **LTV** | Lifetime Value | €500 (5 anni) |
| **CAC** | Customer Acquisition Cost | €20 |
| **LTV/CAC Ratio** | Efficienza acquisizione | 25:1 |
| **Churn Rate** | % utenti persi mensile | < 5% |
| **NPS** | Net Promoter Score | > 50 |

### Revenue Dashboard (Planned)

```
Admin Dashboard ? Revenue Analytics
- Daily revenue graph
- Fee breakdown (deposit, investment, commission)
- Top revenue users
- Consultant performance
- Projected monthly revenue
```

---

## Regulatory Considerations

### MIFID II Compliance
- ? Fee disclosure obbligatoria
- ? Best execution policy
- ? Conflict of interest disclosure

### GDPR
- ? Fee data retention: 10 anni (regulatory)
- ? User può richiedere export fee history

### AML/KYC
- ? Monitoring transazioni anomale
- ? Alert su deposit > €10,000

---

**Autore**: Edoardo Carollo  
**Data**: 2025-01-18  
**Versione**: 1.0.0
