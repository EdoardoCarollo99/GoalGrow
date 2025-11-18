# GoalGrow Setup Script
# Automated setup for development environment

param(
    [string]$ConnectionString = "Server=.;Database=GoalGrowDb;Trusted_Connection=True;TrustServerCertificate=True",
    [switch]$SkipDocker = $false,
    [switch]$ResetDatabase = $false
)

$ErrorActionPreference = "Stop"

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  GoalGrow Development Setup        " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if command exists
function Test-CommandExists {
    param($command)
    $null = Get-Command $command -ErrorAction SilentlyContinue
    return $?
}

# Check prerequisites
Write-Host "?? Checking prerequisites..." -ForegroundColor Yellow
Write-Host ""

$prerequisites = @(
    @{ Name = ".NET 10 SDK"; Command = "dotnet"; CheckArg = "--version" },
    @{ Name = "Docker Desktop"; Command = "docker"; CheckArg = "--version"; Optional = $SkipDocker },
    @{ Name = "Git"; Command = "git"; CheckArg = "--version" }
)

$missingPrerequisites = @()

foreach ($prereq in $prerequisites) {
    if ($prereq.Optional) {
        Write-Host "  ??  Skipping $($prereq.Name) (optional)" -ForegroundColor Gray
        continue
    }
    
    if (Test-CommandExists $prereq.Command) {
        $version = & $prereq.Command $prereq.CheckArg 2>&1 | Select-Object -First 1
        Write-Host "  ? $($prereq.Name): $version" -ForegroundColor Green
    } else {
        Write-Host "  ? $($prereq.Name): NOT FOUND" -ForegroundColor Red
        $missingPrerequisites += $prereq.Name
    }
}

if ($missingPrerequisites.Count -gt 0) {
    Write-Host ""
    Write-Host "? Missing prerequisites:" -ForegroundColor Red
    $missingPrerequisites | ForEach-Object { Write-Host "   - $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "Please install missing tools and try again." -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "? All prerequisites satisfied!" -ForegroundColor Green
Write-Host ""

# Configure User Secrets
Write-Host "?? Configuring User Secrets..." -ForegroundColor Yellow

$migrationProject = Join-Path $PSScriptRoot "GoalGrow.Migration"

if (-not (Test-Path $migrationProject)) {
    Write-Error "GoalGrow.Migration project not found at: $migrationProject"
    exit 1
}

Push-Location $migrationProject

try {
    # Initialize user secrets if not exists
    $secretsInit = dotnet user-secrets list 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  Initializing user secrets..." -ForegroundColor Gray
        dotnet user-secrets init | Out-Null
    }
    
    # Set connection string
    Write-Host "  Setting connection string..." -ForegroundColor Gray
    dotnet user-secrets set "ConnectionStrings:GoalGrowDb" $ConnectionString | Out-Null
    
    Write-Host "  ? User Secrets configured" -ForegroundColor Green
} finally {
    Pop-Location
}

Write-Host ""

# Reset database if requested
if ($ResetDatabase) {
    Write-Host "???  Resetting database..." -ForegroundColor Yellow
    Push-Location $migrationProject
    try {
        dotnet ef database drop --project ../GoalGrow.Data --startup-project . --force | Out-Null
        Write-Host "  ? Database dropped" -ForegroundColor Green
    } catch {
        Write-Host "  ??  Database drop failed (may not exist)" -ForegroundColor Yellow
    } finally {
        Pop-Location
    }
    Write-Host ""
}

# Run migrations and seed
Write-Host "???  Setting up database..." -ForegroundColor Yellow

Push-Location $migrationProject

try {
    Write-Host "  Running migrations..." -ForegroundColor Gray
    dotnet ef database update --project ../GoalGrow.Data --startup-project . | Out-Null
    
    Write-Host "  Seeding sample data..." -ForegroundColor Gray
    dotnet run --no-build | Out-Null
    
    Write-Host "  ? Database setup complete" -ForegroundColor Green
} catch {
    Write-Host "  ? Database setup failed: $_" -ForegroundColor Red
    Pop-Location
    exit 1
} finally {
    Pop-Location
}

Write-Host ""

# Start Keycloak with Docker
if (-not $SkipDocker) {
    Write-Host "?? Starting Keycloak..." -ForegroundColor Yellow
    
    $dockerCompose = Join-Path $PSScriptRoot "docker-compose.yml"
    
    if (-not (Test-Path $dockerCompose)) {
        Write-Host "  ??  docker-compose.yml not found, skipping Keycloak" -ForegroundColor Yellow
    } else {
        try {
            Push-Location $PSScriptRoot
            docker-compose up -d 2>&1 | Out-Null
            
            Write-Host "  ? Keycloak starting..." -ForegroundColor Green
            Write-Host "     Access at: http://localhost:8080" -ForegroundColor Cyan
            Write-Host "     Username: admin" -ForegroundColor Cyan
            Write-Host "     Password: admin" -ForegroundColor Cyan
        } catch {
            Write-Host "  ??  Docker Compose failed: $_" -ForegroundColor Yellow
        } finally {
            Pop-Location
        }
    }
} else {
    Write-Host "??  Skipping Docker setup (--SkipDocker flag)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Setup Complete! ??                " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "? What's been set up:" -ForegroundColor Green
Write-Host "  - User Secrets configured" -ForegroundColor White
Write-Host "  - Database created and migrated" -ForegroundColor White
Write-Host "  - Sample data seeded" -ForegroundColor White
if (-not $SkipDocker) {
    Write-Host "  - Keycloak started (http://localhost:8080)" -ForegroundColor White
}

Write-Host ""
Write-Host "?? Next steps:" -ForegroundColor Cyan
Write-Host "  1. Open solution in Visual Studio" -ForegroundColor White
Write-Host "  2. Explore sample data in SQL Server" -ForegroundColor White
Write-Host "  3. Configure Keycloak realm (see docs/GETTING_STARTED.md)" -ForegroundColor White
Write-Host "  4. Start building the API (see docs/ROADMAP.md)" -ForegroundColor White

Write-Host ""
Write-Host "?? Documentation: docs/INDEX.md" -ForegroundColor Yellow
Write-Host "?? View sample data: SELECT * FROM Users" -ForegroundColor Yellow

Write-Host ""
