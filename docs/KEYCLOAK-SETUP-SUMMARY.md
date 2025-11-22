# ?? Keycloak Setup - Riepilogo Completo

## ?? **Obiettivo**
Verificare che Keycloak sia configurato correttamente prima di implementare le API GoalGrow.

---

## ?? **Documentazione Creata**

| File | Descrizione |
|------|-------------|
| `docs/keycloak-quick-checklist.md` | ? Checklist rapida 1 pagina - **STAMPA QUESTA** |
| `docs/keycloak-pre-api-checklist.md` | ?? Guida completa dettagliata con troubleshooting |
| `Test-KeycloakSetup.ps1` | ?? Script automatico di verifica |

---

## ?? **Quick Start (3 passi)**

### **Passo 1: Stampa Checklist**
```bash
# Apri e stampa per riferimento
docs/keycloak-quick-checklist.md
```

### **Passo 2: Esegui Script Verifica**
```powershell
.\Test-KeycloakSetup.ps1
```

Output esempio:
```
=====================================================
  GoalGrow - Keycloak Verification Tool
=====================================================

[1/8] Verifica Docker...
  ? Keycloak container running

[2/8] Verifica Keycloak raggiungibile...
  ? Keycloak risponde su http://localhost:8080

[3/8] Verifica OpenID Configuration...
  ? Realm 'GoalGrow-Dev' configurato correttamente

...

=====================================================
  RIEPILOGO VERIFICA
=====================================================
  ? Test riusciti:  8
  ? Test falliti:   0

  ?? TUTTO OK! Sei pronto per implementare le API!
```

### **Passo 3: Se Errori ? Segui Checklist Dettagliata**
```bash
# Apri la guida completa
docs/keycloak-pre-api-checklist.md
```

---

## ?? **Checklist Minima (Stampa Questa)**

### ?? **Docker**
- [ ] `docker ps` mostra keycloak running
- [ ] http://localhost:8080 raggiungibile

### ?? **Realm**
- [ ] Realm "GoalGrow-Dev" esiste
- [ ] OpenID config: http://localhost:8080/realms/GoalGrow-Dev/.well-known/openid-configuration

### ?? **Client**
- [ ] Client "goalgrow-api" creato
- [ ] Client authentication: ON
- [ ] Client Secret copiato

### ?? **Roles**
- [ ] investor ?
- [ ] consultant ?
- [ ] admin ?
- [ ] kyc-verified ?

### ?? **Users**
- [ ] admin@goalgrow.com (pass: Admin123!) ? role: admin
- [ ] investor@goalgrow.com (pass: Investor123!) ? role: investor
- [ ] consultant@goalgrow.com (pass: Consultant123!) ? role: consultant

### ?? **User Secrets**
```bash
cd GoalGrow.API
dotnet user-secrets list

# Deve mostrare:
# Keycloak:Authority = http://localhost:8080/realms/GoalGrow-Dev
# Keycloak:ClientSecret = <il-tuo-secret>
# ConnectionStrings:GoalGrowDb = Server=...
```

### ?? **Test Login**
```powershell
# PowerShell
$body = @{
    grant_type = "password"
    client_id = "goalgrow-api"
    client_secret = "YOUR_SECRET"
    username = "admin@goalgrow.com"
    password = "Admin123!"
}

$token = Invoke-RestMethod -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" -Method Post -Body $body -ContentType "application/x-www-form-urlencoded"

# Deve ritornare access_token
$token.access_token
```

### ?? **Test API**
```bash
# Terminal 1: Avvia API
cd GoalGrow.API
dotnet run

# Terminal 2: Test endpoint
curl -H "Authorization: Bearer YOUR_TOKEN" https://localhost:7001/api/auth/verify

# Deve ritornare:
# {
#   "success": true,
#   "data": { "isAuthenticated": true, ... }
# }
```

---

## ? **Se Tutto OK ? Prossimi Passi**

### **1. Implementare IUserService** (2-3 ore)
File da creare:
- `GoalGrow.API/Services/Interfaces/IUserService.cs`
- `GoalGrow.API/Services/Implementations/UserService.cs`

Funzionalità:
- `GetOrCreateUserAsync(ClaimsPrincipal)` - Sync Keycloak ? DB
- `UpdateUserProfileAsync()`
- `GetUserByIdAsync()`

### **2. Implementare UsersController** (1 ora)
File da creare:
- `GoalGrow.API/Controllers/UsersController.cs`

Endpoints:
- `GET /api/users/me` - Profilo utente corrente
- `PUT /api/users/me` - Aggiorna profilo

### **3. Test con Postman** (30 min)
Collection:
1. Login ? Get token
2. GET /api/users/me ? Verifica utente
3. PUT /api/users/me ? Aggiorna nome

---

## ?? **Troubleshooting Veloce**

| Errore | Fix Rapido |
|--------|-----------|
| Container not found | `docker-compose up -d` |
| Port 8080 already in use | Cambia porta in docker-compose.yml |
| Realm not found | Crea realm "GoalGrow-Dev" in admin console |
| Invalid client | Verifica Client ID = "goalgrow-api" |
| Wrong client secret | Ricopia da Keycloak ? Clients ? goalgrow-api ? Credentials |
| User Secrets empty | `dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080/realms/GoalGrow-Dev"` |
| Build errors | `dotnet clean && dotnet build` |

---

## ?? **Supporto**

Se lo script `Test-KeycloakSetup.ps1` mostra errori:

1. **Controlla output dello script** - indica esattamente cosa manca
2. **Segui checklist dettagliata** in `docs/keycloak-pre-api-checklist.md`
3. **Verifica logs Keycloak**: `docker-compose logs -f keycloak`

---

## ?? **Stato Attuale del Progetto**

### ? Completato
- [x] Database schema con TPH inheritance
- [x] Migration AddKeycloakSubjectId applicata
- [x] Warning CS8625 risolti
- [x] AuthController con JWT validation
- [x] Keycloak checklist e script di verifica

### ?? Next Steps
- [ ] Configurare Keycloak (segui checklist)
- [ ] Implementare UserService
- [ ] Implementare UsersController
- [ ] Test endpoint /api/users/me

### ?? Branch Attuale
```
developing/authenticationAPI
```

### ?? Obiettivo Branch
Autenticazione JWT + sincronizzazione User ? Keycloak funzionante

---

## ?? **Tips**

1. **Usa lo script automatico** - risparmia tempo
2. **Stampa la quick checklist** - tienila vicino mentre configuri
3. **Testa subito dopo ogni step** - non aspettare la fine
4. **Salva il Client Secret** - ti servirà spesso
5. **Usa Postman Collections** - salva le request per riutilizzarle

---

**Buona configurazione! ??**
