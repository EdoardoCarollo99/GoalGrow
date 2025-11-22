# ?? Keycloak Pre-API Verification Checklist

## ?? **Checklist Completa - Prima delle API**

Usa questa checklist per verificare che Keycloak sia configurato correttamente prima di implementare le API.

---

## ? **Fase 1: Infrastruttura Keycloak**

### 1.1 Docker Setup
- [ ] **Docker Desktop installato e funzionante**
  ```bash
  docker --version
  # Dovrebbe mostrare: Docker version 20.x o superiore
  ```

- [ ] **Keycloak container running**
  ```bash
  docker ps | findstr keycloak
  # Dovrebbe mostrare un container keycloak in stato UP
  ```

- [ ] **Keycloak accessibile su http://localhost:8080**
  - Apri browser: `http://localhost:8080`
  - Login admin console: `http://localhost:8080/admin`
  - Username: `admin`
  - Password: `admin` (o la tua password configurata)

### 1.2 Verifica docker-compose.yml
- [ ] **File docker-compose.yml esiste nella root del progetto**
  - Se NON esiste, crearlo (vedi sezione "Setup Docker Compose" sotto)

---

## ? **Fase 2: Configurazione Realm**

### 2.1 Realm GoalGrow-Dev
- [ ] **Realm "GoalGrow-Dev" creato**
  - Login Keycloak admin: `http://localhost:8080/admin`
  - Verifica in alto a sinistra: dropdown realm mostra "GoalGrow-Dev"
  - Se NON esiste:
    - Click su "master" ? "Create Realm"
    - Nome: `GoalGrow-Dev`
    - Enabled: `ON`

### 2.2 Verifica Realm Settings
- [ ] **Realm Settings ? General**
  - Nome: `GoalGrow-Dev`
  - Enabled: `ON`
  - User-managed access: `OFF` (per ora)

- [ ] **Realm Settings ? Login**
  - User registration: `ON`
  - Forgot password: `ON`
  - Remember me: `ON`
  - Email as username: `ON` (consigliato per GoalGrow)

- [ ] **Realm Settings ? Tokens**
  - SSO Session Idle: `30 Minutes`
  - SSO Session Max: `10 Hours`
  - Access Token Lifespan: `5 Minutes`
  - Refresh Token Max Reuse: `0`

---

## ? **Fase 3: Client Configuration**

### 3.1 Client: goalgrow-api (Backend)
- [ ] **Client "goalgrow-api" creato**
  - Clients ? Create client
  - Client type: `OpenID Connect`
  - Client ID: `goalgrow-api`
  - Name: `GoalGrow API Backend`

- [ ] **Configurazione Client goalgrow-api**
  - **Settings Tab:**
    - Client authentication: `ON` (Confidential)
    - Authorization: `OFF`
    - Standard flow: `ON`
    - Direct access grants: `ON`
    - Implicit flow: `OFF`
    - Service accounts: `ON`
    - OAuth 2.0 Device Authorization: `OFF`
  
  - **Access Settings:**
    - Root URL: `https://localhost:7001`
    - Valid redirect URIs: 
      - `https://localhost:7001/*`
      - `http://localhost:5000/*`
    - Valid post logout redirect URIs: `+`
    - Web origins: 
      - `https://localhost:7001`
      - `http://localhost:5000`

- [ ] **Client Secret salvato**
  - Tab "Credentials"
  - Copia il "Client Secret"
  - Salvalo in User Secrets (vedi Fase 5)

### 3.2 Client: account (Built-in)
- [ ] **Verifica client "account" esiste**
  - Clients ? account
  - Questo è il client built-in di Keycloak per gestione account
  - NON modificare, serve per login/logout standard

---

## ? **Fase 4: Roles Configuration**

### 4.1 Realm Roles
- [ ] **Role "investor" creata**
  - Realm roles ? Create role
  - Role name: `investor`
  - Description: `Investitore - Utente che risparmia e investe`

- [ ] **Role "consultant" creata**
  - Role name: `consultant`
  - Description: `Consulente finanziario`

- [ ] **Role "admin" creata**
  - Role name: `admin`
  - Description: `Amministratore piattaforma`

- [ ] **Role "kyc-verified" creata**
  - Role name: `kyc-verified`
  - Description: `Utente con KYC completato`

### 4.2 Client Roles (goalgrow-api)
- [ ] **Verifica client roles per goalgrow-api**
  - Clients ? goalgrow-api ? Roles
  - Dovrebbero esserci ruoli di default
  - NON servono ruoli custom per ora

---

## ? **Fase 5: User Secrets Configuration**

### 5.1 Navigazione nel progetto API
```bash
cd C:\Users\edoardo.carollo\source\repos\Personale\GoalGrow\GoalGrow.API
```

### 5.2 Inizializzazione User Secrets
- [ ] **User Secrets inizializzati**
  ```bash
  dotnet user-secrets init
  ```

### 5.3 Configurazione Secrets
- [ ] **Keycloak Authority configurata**
  ```bash
  dotnet user-secrets set "Keycloak:Authority" "http://localhost:8080/realms/GoalGrow-Dev"
  ```

- [ ] **Client Secret configurato** (usa il secret copiato da Keycloak)
  ```bash
  dotnet user-secrets set "Keycloak:ClientSecret" "IL_TUO_CLIENT_SECRET_QUI"
  ```

- [ ] **Connection String configurata**
  ```bash
  dotnet user-secrets set "ConnectionStrings:GoalGrowDb" "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True"
  ```

### 5.4 Verifica User Secrets
- [ ] **Lista secrets configurati**
  ```bash
  dotnet user-secrets list
  ```
  
  Dovresti vedere:
  ```
  Keycloak:Authority = http://localhost:8080/realms/GoalGrow-Dev
  Keycloak:ClientSecret = <il-tuo-secret>
  ConnectionStrings:GoalGrowDb = Server=.;Database=GoalGrowDb;...
  ```

---

## ? **Fase 6: Test Users**

### 6.1 Utente Admin
- [ ] **User admin creato**
  - Users ? Add user
  - Username: `admin@goalgrow.com`
  - Email: `admin@goalgrow.com`
  - First name: `Admin`
  - Last name: `GoalGrow`
  - Email verified: `ON`
  - Enabled: `ON`

- [ ] **Password admin impostata**
  - User ? Credentials ? Set password
  - Password: `Admin123!` (o la tua password)
  - Temporary: `OFF`

- [ ] **Ruolo admin assegnato**
  - User ? Role mappings ? Assign role
  - Filter: "admin"
  - Seleziona "admin" ? Assign

### 6.2 Utente Investor
- [ ] **User investor creato**
  - Username: `investor@goalgrow.com`
  - Email: `investor@goalgrow.com`
  - First name: `Investor`
  - Last name: `Test`
  - Password: `Investor123!`
  - Role: `investor`

### 6.3 Utente Consultant
- [ ] **User consultant creato**
  - Username: `consultant@goalgrow.com`
  - Email: `consultant@goalgrow.com`
  - First name: `Consultant`
  - Last name: `Test`
  - Password: `Consultant123!`
  - Roles: `consultant`

---

## ? **Fase 7: Verifica Configurazione API**

### 7.1 File appsettings.json
- [ ] **Verifica GoalGrow.API/appsettings.json**
  ```json
  {
    "Keycloak": {
      "Audience": "goalgrow-api",
      "ClientId": "goalgrow-api",
      "RequireHttpsMetadata": false,
      "ValidateIssuer": true,
      "ValidateAudience": true,
      "ValidateLifetime": true,
      "ClockSkew": 0
    }
  }
  ```

### 7.2 Program.cs
- [ ] **Verifica GoalGrow.API/Program.cs ha JWT authentication**
  - ? `AddAuthentication(JwtBearerDefaults.AuthenticationScheme)`
  - ? `AddJwtBearer` configurato
  - ? `UseAuthentication()` prima di `UseAuthorization()`

---

## ? **Fase 8: Test Connessione**

### 8.1 Test Keycloak OIDC Endpoint
- [ ] **Test OpenID Configuration**
  ```bash
  # PowerShell
  Invoke-WebRequest "http://localhost:8080/realms/GoalGrow-Dev/.well-known/openid-configuration" | Select-Object -ExpandProperty Content
  ```
  
  Dovresti vedere JSON con:
  - `issuer`: `http://localhost:8080/realms/GoalGrow-Dev`
  - `authorization_endpoint`
  - `token_endpoint`
  - `jwks_uri`

### 8.2 Test Login
- [ ] **Test login con Postman/cURL**
  ```bash
  # PowerShell - Request token
  $body = @{
      grant_type = "password"
      client_id = "goalgrow-api"
      client_secret = "IL_TUO_CLIENT_SECRET"
      username = "admin@goalgrow.com"
      password = "Admin123!"
  }

  $response = Invoke-RestMethod -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" -Method Post -Body $body -ContentType "application/x-www-form-urlencoded"

  $response.access_token
  ```

  Dovresti ricevere:
  - `access_token` (JWT)
  - `refresh_token`
  - `expires_in`
  - `token_type`: "Bearer"

### 8.3 Test API con JWT
- [ ] **Avvia API**
  ```bash
  cd GoalGrow.API
  dotnet run
  ```

- [ ] **Test endpoint protetto**
  ```bash
  # Con il token ricevuto sopra
  $token = "IL_TUO_ACCESS_TOKEN"
  
  Invoke-RestMethod -Uri "https://localhost:7001/api/auth/verify" -Headers @{Authorization="Bearer $token"}
  ```

  Dovresti ricevere:
  ```json
  {
    "success": true,
    "message": "Token is valid",
    "data": {
      "userId": "...",
      "username": "admin@goalgrow.com",
      "email": "admin@goalgrow.com",
      "isAuthenticated": true
    }
  }
  ```

---

## ? **Fase 9: Troubleshooting Comune**

### Problema: "Unable to obtain configuration from..."
**Soluzione:**
- [ ] Verifica Keycloak running: `docker ps`
- [ ] Verifica URL Authority corretta in User Secrets
- [ ] Test endpoint: `http://localhost:8080/realms/GoalGrow-Dev/.well-known/openid-configuration`

### Problema: "Invalid client credentials"
**Soluzione:**
- [ ] Verifica Client Secret corretto in User Secrets
- [ ] Ricontrolla in Keycloak: Clients ? goalgrow-api ? Credentials

### Problema: "Audience validation failed"
**Soluzione:**
- [ ] Verifica `Audience` in appsettings.json = `goalgrow-api`
- [ ] Verifica in Keycloak: Clients ? goalgrow-api ? Settings ? Client ID

### Problema: "Issuer validation failed"
**Soluzione:**
- [ ] Verifica Authority in User Secrets
- [ ] Deve essere: `http://localhost:8080/realms/GoalGrow-Dev`
- [ ] NON deve avere slash finale

---

## ? **Fase 10: Checklist Finale Pre-API**

Prima di iniziare con le API, verifica:

- [ ] ? Keycloak running su http://localhost:8080
- [ ] ? Realm "GoalGrow-Dev" creato
- [ ] ? Client "goalgrow-api" configurato
- [ ] ? Roles (investor, consultant, admin, kyc-verified) create
- [ ] ? Test users (admin, investor, consultant) creati
- [ ] ? User Secrets configurati correttamente
- [ ] ? Test login con Postman funziona
- [ ] ? Test `/api/auth/verify` con JWT funziona
- [ ] ? Build progetto API successful
- [ ] ? Database GoalGrowDb esistente e aggiornato

---

## ?? **Setup Docker Compose (se mancante)**

Se non hai `docker-compose.yml`, crea questo file nella root del progetto:

```yaml
version: '3.8'

services:
  keycloak:
    image: quay.io/keycloak/keycloak:23.0
    container_name: goalgrow-keycloak
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: keycloak
      KC_HOSTNAME_STRICT: false
      KC_HTTP_ENABLED: true
    ports:
      - "8080:8080"
    command: start-dev
    depends_on:
      - postgres
    networks:
      - goalgrow-network

  postgres:
    image: postgres:15-alpine
    container_name: goalgrow-postgres
    environment:
      POSTGRES_DB: keycloak
      POSTGRES_USER: keycloak
      POSTGRES_PASSWORD: keycloak
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - goalgrow-network

volumes:
  postgres_data:

networks:
  goalgrow-network:
    driver: bridge
```

**Avvio:**
```bash
docker-compose up -d
```

**Stop:**
```bash
docker-compose down
```

**Logs:**
```bash
docker-compose logs -f keycloak
```

---

## ?? **Pronto per le API!**

Se tutte le checkbox sono ?, sei pronto per implementare:
- `IUserService` e `UserService`
- `UsersController`
- Endpoint `/api/users/me`
- Sincronizzazione User ? Keycloak

**Prossimo step:** Implementazione API Layer! ??
