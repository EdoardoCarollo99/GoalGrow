# ====================================================
# GoalGrow - Keycloak Verification Script
# ====================================================
# Questo script verifica che Keycloak sia configurato correttamente

param(
    [string]$KeycloakUrl = "http://localhost:8080",
    [string]$Realm = "GoalGrow-Dev",
    [string]$ClientId = "goalgrow-api"
)

Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "  GoalGrow - Keycloak Verification Tool" -ForegroundColor Cyan
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host ""

$ErrorCount = 0
$SuccessCount = 0

# ====================================================
# Test 1: Docker Running
# ====================================================
Write-Host "[1/8] Verifica Docker..." -ForegroundColor Yellow

try {
    $dockerPs = docker ps 2>&1
    if ($dockerPs -like "*keycloak*") {
        Write-Host "  ? Keycloak container running" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "  ? Keycloak container NON trovato" -ForegroundColor Red
        Write-Host "    Esegui: docker-compose up -d" -ForegroundColor Yellow
        $ErrorCount++
    }
} catch {
    Write-Host "  ? Docker non disponibile" -ForegroundColor Red
    Write-Host "    Verifica che Docker Desktop sia avviato" -ForegroundColor Yellow
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 2: Keycloak Reachable
# ====================================================
Write-Host "[2/8] Verifica Keycloak raggiungibile..." -ForegroundColor Yellow

try {
    $response = Invoke-WebRequest -Uri "$KeycloakUrl" -TimeoutSec 5 -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "  ? Keycloak risponde su $KeycloakUrl" -ForegroundColor Green
        $SuccessCount++
    }
} catch {
    Write-Host "  ? Keycloak NON raggiungibile su $KeycloakUrl" -ForegroundColor Red
    Write-Host "    Errore: $($_.Exception.Message)" -ForegroundColor Yellow
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 3: OpenID Configuration
# ====================================================
Write-Host "[3/8] Verifica OpenID Configuration..." -ForegroundColor Yellow

try {
    $oidcUrl = "$KeycloakUrl/realms/$Realm/.well-known/openid-configuration"
    $oidcConfig = Invoke-RestMethod -Uri $oidcUrl -ErrorAction Stop
    
    if ($oidcConfig.issuer -eq "$KeycloakUrl/realms/$Realm") {
        Write-Host "  ? Realm '$Realm' configurato correttamente" -ForegroundColor Green
        Write-Host "    Issuer: $($oidcConfig.issuer)" -ForegroundColor Gray
        $SuccessCount++
    } else {
        Write-Host "  ? Issuer non corrisponde" -ForegroundColor Red
        $ErrorCount++
    }
} catch {
    Write-Host "  ? Realm '$Realm' NON trovato" -ForegroundColor Red
    Write-Host "    Crea il realm in Keycloak Admin Console" -ForegroundColor Yellow
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 4: User Secrets
# ====================================================
Write-Host "[4/8] Verifica User Secrets..." -ForegroundColor Yellow

try {
    Push-Location "GoalGrow.API"
    
    $secrets = dotnet user-secrets list 2>&1 | Out-String
    
    if ($secrets -match "Keycloak:Authority") {
        Write-Host "  ? Keycloak:Authority configurata" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "  ? Keycloak:Authority mancante" -ForegroundColor Red
        Write-Host "    Esegui: dotnet user-secrets set 'Keycloak:Authority' '$KeycloakUrl/realms/$Realm'" -ForegroundColor Yellow
        $ErrorCount++
    }
    
    if ($secrets -match "Keycloak:ClientSecret") {
        Write-Host "  ? Keycloak:ClientSecret configurato" -ForegroundColor Green
    } else {
        Write-Host "  ? Keycloak:ClientSecret mancante" -ForegroundColor Yellow
        Write-Host "    Prendi il secret da Keycloak e configuralo" -ForegroundColor Yellow
    }
    
    if ($secrets -match "ConnectionStrings:GoalGrowDb") {
        Write-Host "  ? ConnectionString configurata" -ForegroundColor Green
    } else {
        Write-Host "  ? ConnectionString mancante" -ForegroundColor Yellow
    }
    
    Pop-Location
} catch {
    Write-Host "  ? Errore verifica User Secrets" -ForegroundColor Red
    Pop-Location
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 5: Database
# ====================================================
Write-Host "[5/8] Verifica Database..." -ForegroundColor Yellow

try {
    Push-Location "GoalGrow.Migration"
    
    # Prova a connettersi al DB (verifica migrazione più recente)
    $migrations = dotnet ef migrations list --no-build 2>&1 | Out-String
    
    if ($migrations -match "AddKeycloakSubjectId") {
        Write-Host "  ? Migration AddKeycloakSubjectId trovata" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "  ? Migration AddKeycloakSubjectId non applicata" -ForegroundColor Yellow
    }
    
    Pop-Location
} catch {
    Write-Host "  ? Errore verifica database" -ForegroundColor Red
    Pop-Location
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 6: Build Solution
# ====================================================
Write-Host "[6/8] Verifica Build..." -ForegroundColor Yellow

try {
    $buildResult = dotnet build --nologo --verbosity quiet 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  ? Build successful" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "  ? Build failed" -ForegroundColor Red
        $ErrorCount++
    }
} catch {
    Write-Host "  ? Errore durante build" -ForegroundColor Red
    $ErrorCount++
}

Write-Host ""

# ====================================================
# Test 7: Test Login (se Client Secret configurato)
# ====================================================
Write-Host "[7/8] Test Login..." -ForegroundColor Yellow

try {
    Push-Location "GoalGrow.API"
    
    $secrets = dotnet user-secrets list 2>&1 | Out-String
    
    if ($secrets -match "Keycloak:ClientSecret = (.+)") {
        $clientSecret = $Matches[1].Trim()
        
        Write-Host "  Tentativo login con utente test..." -ForegroundColor Gray
        
        $body = @{
            grant_type = "password"
            client_id = $ClientId
            client_secret = $clientSecret
            username = "admin@goalgrow.com"
            password = "Admin123!"
        }
        
        try {
            $tokenUrl = "$KeycloakUrl/realms/$Realm/protocol/openid-connect/token"
            $response = Invoke-RestMethod -Uri $tokenUrl -Method Post -Body $body -ContentType "application/x-www-form-urlencoded" -ErrorAction Stop
            
            if ($response.access_token) {
                Write-Host "  ? Login successful - Token ricevuto" -ForegroundColor Green
                Write-Host "    Username: admin@goalgrow.com" -ForegroundColor Gray
                $SuccessCount++
            }
        } catch {
            Write-Host "  ? Login fallito (utente potrebbe non esistere)" -ForegroundColor Yellow
            Write-Host "    Crea l'utente admin@goalgrow.com in Keycloak" -ForegroundColor Yellow
        }
    } else {
        Write-Host "  ? Skip - Client Secret non configurato" -ForegroundColor Gray
    }
    
    Pop-Location
} catch {
    Write-Host "  ? Errore test login" -ForegroundColor Red
    Pop-Location
}

Write-Host ""

# ====================================================
# Test 8: API Endpoint
# ====================================================
Write-Host "[8/8] Verifica API Endpoints..." -ForegroundColor Yellow

try {
    # Verifica che Program.cs abbia authentication configurata
    $programCs = Get-Content "GoalGrow.API\Program.cs" -Raw
    
    if ($programCs -match "AddAuthentication" -and $programCs -match "AddJwtBearer") {
        Write-Host "  ? JWT Authentication configurata in Program.cs" -ForegroundColor Green
        $SuccessCount++
    } else {
        Write-Host "  ? JWT Authentication NON configurata" -ForegroundColor Red
        $ErrorCount++
    }
    
    if ($programCs -match "UseAuthentication" -and $programCs -match "UseAuthorization") {
        Write-Host "  ? Middleware authentication/authorization configurati" -ForegroundColor Green
    } else {
        Write-Host "  ? Middleware authentication mancanti" -ForegroundColor Red
    }
    
} catch {
    Write-Host "  ? Errore verifica API configuration" -ForegroundColor Red
}

Write-Host ""

# ====================================================
# Summary
# ====================================================
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "  RIEPILOGO VERIFICA" -ForegroundColor Cyan
Write-Host "=====================================================" -ForegroundColor Cyan

Write-Host "  ? Test riusciti:  $SuccessCount" -ForegroundColor Green
Write-Host "  ? Test falliti:   $ErrorCount" -ForegroundColor Red

Write-Host ""

if ($ErrorCount -eq 0) {
    Write-Host "  ?? TUTTO OK! Sei pronto per implementare le API!" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Prossimi passi:" -ForegroundColor Yellow
    Write-Host "  1. Implementa IUserService e UserService" -ForegroundColor Gray
    Write-Host "  2. Crea UsersController" -ForegroundColor Gray
    Write-Host "  3. Testa endpoint /api/users/me" -ForegroundColor Gray
} elseif ($ErrorCount -le 2) {
    Write-Host "  ?? Alcuni problemi minori trovati" -ForegroundColor Yellow
    Write-Host "  Risolvi i warning sopra e riprova" -ForegroundColor Yellow
} else {
    Write-Host "  ? Problemi critici trovati" -ForegroundColor Red
    Write-Host "  Segui la checklist in docs/keycloak-pre-api-checklist.md" -ForegroundColor Red
}

Write-Host ""
Write-Host "=====================================================" -ForegroundColor Cyan
