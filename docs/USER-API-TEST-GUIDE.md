# ?? UserService & UsersController - Test Guide

## ?? **Test End-to-End - User API**

Questa guida ti aiuterà a testare la sincronizzazione Keycloak ? Database e gli endpoint User API.

---

## ?? **Step 1: Avvia l'API**

```bash
cd GoalGrow.API
dotnet run
```

**Output atteso:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

---

## ?? **Step 2: Ottieni JWT Token da Keycloak**

### **Opzione A: PowerShell**

```powershell
# Login con admin@goalgrow.com
$body = @{
    grant_type = "password"
    client_id = "goalgrow-api"
    client_secret = "L76lhUEKgudHRkj73B03O2ev5SuURrju"
    username = "admin@goalgrow.com"
    password = "Admin123!"
}

$response = Invoke-RestMethod -Uri "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" -Method Post -Body $body -ContentType "application/x-www-form-urlencoded"

# Salva il token
$token = $response.access_token
Write-Host "Access Token: $token"
```

### **Opzione B: cURL**

```bash
curl -X POST "http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=goalgrow-api" \
  -d "client_secret=L76lhUEKgudHRkj73B03O2ev5SuURrju" \
  -d "username=admin@goalgrow.com" \
  -d "password=Admin123!"
```

### **Opzione C: Postman**

1. **Create Request:**
   - Method: `POST`
   - URL: `http://localhost:8080/realms/GoalGrow-Dev/protocol/openid-connect/token`

2. **Body (x-www-form-urlencoded):**
   ```
   grant_type: password
   client_id: goalgrow-api
   client_secret: L76lhUEKgudHRkj73B03O2ev5SuURrju
   username: admin@goalgrow.com
   password: Admin123!
   ```

3. **Send** ? Copia `access_token` dalla response

---

## ?? **Step 3: Test GET /api/users/me**

### **PowerShell**

```powershell
# Usa il token dell'Step 2
$headers = @{
    Authorization = "Bearer $token"
}

$response = Invoke-RestMethod -Uri "https://localhost:7001/api/users/me" -Headers $headers

# Mostra response
$response | ConvertTo-Json -Depth 10
```

### **cURL**

```bash
curl -X GET "https://localhost:7001/api/users/me" \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -k
```

### **Postman**

1. **Create Request:**
   - Method: `GET`
   - URL: `https://localhost:7001/api/users/me`

2. **Authorization:**
   - Type: `Bearer Token`
   - Token: `<IL_TUO_ACCESS_TOKEN>`

3. **Send**

### **Expected Response:**

```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "firstName": "Admin",
    "lastName": "GoalGrow",
    "emailAddress": "admin@goalgrow.com",
    "phoneNumber": "",
    "userType": "Admin",
    "keycloakSubjectId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "createdAt": "2024-11-22T14:30:00Z"
  },
  "errors": null,
  "timestamp": "2024-11-22T14:30:00Z"
}
```

---

## ?? **Step 4: Verifica Database**

### **SQL Query**

```sql
-- Verifica che l'utente sia stato creato con KeycloakSubjectId
SELECT 
    Id,
    FirstName,
    LastName,
    EmailAddress,
    UserType,
    KeycloakSubjectId
FROM Users
WHERE EmailAddress = 'admin@goalgrow.com';
```

**Expected Result:**

| Id | FirstName | LastName | EmailAddress | UserType | KeycloakSubjectId |
|----|-----------|----------|--------------|----------|-------------------|
| (GUID) | Admin | GoalGrow | admin@goalgrow.com | 3 | f47ac10b-... |

? **KeycloakSubjectId dovrebbe essere popolato!**

---

## ?? **Step 5: Test PUT /api/users/me**

### **PowerShell**

```powershell
$updateBody = @{
    firstName = "Admin Updated"
    lastName = "GoalGrow Modified"
    phoneNumber = "+39 123 456 7890"
} | ConvertTo-Json

$headers = @{
    Authorization = "Bearer $token"
    "Content-Type" = "application/json"
}

$response = Invoke-RestMethod -Uri "https://localhost:7001/api/users/me" -Method Put -Headers $headers -Body $updateBody

$response | ConvertTo-Json -Depth 10
```

### **Postman**

1. **Create Request:**
   - Method: `PUT`
   - URL: `https://localhost:7001/api/users/me`

2. **Authorization:**
   - Type: `Bearer Token`
   - Token: `<IL_TUO_ACCESS_TOKEN>`

3. **Body (JSON):**
   ```json
   {
     "firstName": "Admin Updated",
     "lastName": "GoalGrow Modified",
     "phoneNumber": "+39 123 456 7890"
   }
   ```

4. **Send**

### **Expected Response:**

```json
{
  "success": true,
  "message": "Profile updated successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "firstName": "Admin Updated",
    "lastName": "GoalGrow Modified",
    "emailAddress": "admin@goalgrow.com",
    "phoneNumber": "+39 123 456 7890",
    "userType": "Admin",
    "keycloakSubjectId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
    "createdAt": "2024-11-22T14:30:00Z"
  }
}
```

---

## ?? **Step 6: Test con Investor User**

Ripeti gli Step 2-5 ma con:

```powershell
$body = @{
    grant_type = "password"
    client_id = "goalgrow-api"
    client_secret = "L76lhUEKgudHRkj73B03O2ev5SuURrju"
    username = "investor@goalgrow.com"
    password = "Investor123!"
}
```

**Expected Response per Investor:**

```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": "...",
    "firstName": "Investor",
    "lastName": "Test",
    "emailAddress": "investor@goalgrow.com",
    "userType": "Investor",
    "keycloakSubjectId": "...",
    "fiscalCode": "TEMP-XXXXXXXX",
    "birthDate": "1999-11-22T00:00:00Z",
    "virtualWalletBalance": 0.00,
    "totalDeposited": 0.00,
    "totalWithdrawn": 0.00,
    "totalInvested": 0.00,
    "hasKycVerification": false,
    "kycStatus": null
  }
}
```

---

## ?? **Step 7: Test con Consultant User**

```powershell
$body = @{
    grant_type = "password"
    client_id = "goalgrow-api"
    client_secret = "L76lhUEKgudHRkj73B03O2ev5SuURrju"
    username = "consultant@goalgrow.com"
    password = "Consultant123!"
}
```

**Expected Response per Consultant:**

```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": "...",
    "firstName": "Consultant",
    "lastName": "Test",
    "emailAddress": "consultant@goalgrow.com",
    "userType": "Consultant",
    "keycloakSubjectId": "...",
    "licenseNumber": "TEMP-XXXXXXXX",
    "specialization": "",
    "commissionRate": 0.00,
    "yearsOfExperience": 0,
    "averageRating": 0.00,
    "totalReviews": 0
  }
}
```

---

## ? **Checklist Verifica**

### **Test Positivi**

- [ ] ? Login Keycloak funziona per admin
- [ ] ? GET /api/users/me ritorna profilo admin
- [ ] ? Database: admin ha KeycloakSubjectId popolato
- [ ] ? PUT /api/users/me aggiorna profilo
- [ ] ? Login Keycloak funziona per investor
- [ ] ? GET /api/users/me ritorna profilo investor (con campi extra)
- [ ] ? Login Keycloak funziona per consultant
- [ ] ? GET /api/users/me ritorna profilo consultant (con campi extra)

### **Test Negativi (Errori Attesi)**

- [ ] ? GET /api/users/me senza token ? 401 Unauthorized
- [ ] ? PUT /api/users/me senza token ? 401 Unauthorized
- [ ] ? PUT /api/users/me con body invalido ? 400 Bad Request

---

## ?? **Troubleshooting**

### Errore: "Unauthorized"

**Causa:** Token scaduto o invalido

**Soluzione:**
1. Richiedi nuovo token (Step 2)
2. Verifica che Keycloak sia running: `docker ps | findstr keycloak`

### Errore: "Cannot open database"

**Causa:** Connection string errata

**Soluzione:**
```bash
cd GoalGrow.API
dotnet user-secrets list
```

Verifica `ConnectionStrings:GoalGrowDb`

### Errore: "User not found"

**Causa:** Utente non esiste in Keycloak

**Soluzione:**
1. Accedi a Keycloak admin: http://localhost:8080/admin
2. Verifica che l'utente esista in Realm "GoalGrow-Dev"

### Errore: "Invalid client credentials"

**Causa:** Client Secret errato

**Soluzione:**
1. Keycloak Admin ? Clients ? goalgrow-api ? Credentials
2. Copia Client Secret
3. Aggiorna User Secrets:
   ```bash
   dotnet user-secrets set "Keycloak:ClientSecret" "NEW_SECRET"
   ```

---

## ?? **Database Verification Queries**

### **Verifica tutti gli utenti Keycloak**

```sql
SELECT 
    Id,
    FirstName,
    LastName,
    EmailAddress,
    UserType,
    KeycloakSubjectId,
    CASE 
        WHEN KeycloakSubjectId IS NOT NULL THEN 'Synced'
        ELSE 'Not Synced'
    END AS SyncStatus
FROM Users
ORDER BY EmailAddress;
```

### **Conta utenti per tipo**

```sql
SELECT 
    UserType,
    COUNT(*) AS Total,
    SUM(CASE WHEN KeycloakSubjectId IS NOT NULL THEN 1 ELSE 0 END) AS Synced,
    SUM(CASE WHEN KeycloakSubjectId IS NULL THEN 1 ELSE 0 END) AS NotSynced
FROM Users
GROUP BY UserType;
```

---

## ?? **Expected Final State**

Dopo aver testato tutti e 3 gli utenti Keycloak:

```sql
SELECT EmailAddress, KeycloakSubjectId FROM Users WHERE KeycloakSubjectId IS NOT NULL;
```

**Output atteso:**

| EmailAddress | KeycloakSubjectId |
|--------------|-------------------|
| admin@goalgrow.com | f47ac10b-... |
| investor@goalgrow.com | 8c3d4e5f-... |
| consultant@goalgrow.com | 9d4e5f6g-... |

? **Tutti gli utenti Keycloak sincronizzati con il database!**

---

## ?? **Logs da Controllare**

Quando esegui le API call, controlla i logs della console API:

```
info: GoalGrow.API.Services.Implementations.UserService[0]
      Syncing user with Keycloak SubjectId: f47ac10b-..., Email: admin@goalgrow.com
info: GoalGrow.API.Services.Implementations.UserService[0]
      Creating new user from Keycloak claims
info: GoalGrow.API.Services.Implementations.UserService[0]
      Creating AdminUser for admin@goalgrow.com
info: GoalGrow.API.Services.Implementations.UserService[0]
      New user created: a1b2c3d4-..., Type: AdminUser
```

---

## ? **Test Completati con Successo?**

Se tutti i test sopra passano:

**?? CONGRATULAZIONI! UserService & UsersController funzionano correttamente!**

Prossimi step:
1. Commit & push delle modifiche
2. Implementare GoalService
3. Implementare KycService

---

**Buon testing! ??**
