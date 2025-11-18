#  GoalGrow - Implementation Checklist

##  Overview
Use this checklist to track progress on implementing GoalGrow MVP.

---

## Phase 1: Foundation & Refactoring

### Database Refactoring
- [ ] Run `Migrate-Configurations.ps1 -Backup`
- [ ] Verify configuration files moved correctly
- [ ] Update all configuration namespaces
- [ ] Build solution: `dotnet build` (no errors)
- [ ] Run existing migrations to verify DB still works

### New Entity Models
- [x] Create `KycVerification.cs`
- [x] Create `PlatformFee.cs`
- [x] Create `KycStatus.cs` enum
- [x] Create `PlatformFeeType.cs` enum
- [x] Create `GoalType.cs` enum
- [x] Update `Goal.cs` with system goals

### Database Schema Updates
- [ ] Add `KeycloakSubjectId` to `User` table
- [ ] Update `InversotorUser` with `KycVerification` relationship
- [ ] Update `Goal` with new fields (Type, IsSystemGoal, etc.)
- [ ] Create `KycVerifications` table
- [ ] Create `PlatformFees` table

### EF Core Configurations
- [ ] Create `Configurations/Compliance/KycVerificationConfiguration.cs`
- [ ] Create `Configurations/Financial/PlatformFeeConfiguration.cs`
- [ ] Update `GoalConfiguration.cs` for new fields
- [ ] Update `UserConfiguration.cs` for KeycloakSubjectId

### Migrations
- [ ] Create migration: `dotnet ef migrations add AddKycAndSystemGoals`
- [ ] Review generated migration code
- [ ] Apply migration: `dotnet ef database update`
- [ ] Verify all tables created correctly
- [ ] Test rollback: `dotnet ef database update <PreviousMigration>`
- [ ] Re-apply: `dotnet ef database update`

---

## Phase 2: Authentication & Authorization

### Keycloak Setup
- [ ] Install Docker Desktop
- [ ] Create `docker-compose.yml` for Keycloak + Postgres
- [ ] Run `docker-compose up -d`
- [ ] Access Keycloak admin console (http://localhost:8080)
- [ ] Create realm: `GoalGrow-Dev`

### Keycloak Configuration
- [ ] Create client: `goalgrow-api`
  - [ ] Client Protocol: openid-connect
  - [ ] Access Type: confidential
  - [ ] Valid Redirect URIs: `https://localhost:5001/*`
  - [ ] Web Origins: `https://localhost:5001`
- [ ] Create client: `goalgrow-web`
  - [ ] Client Protocol: openid-connect
  - [ ] Access Type: public
  - [ ] Valid Redirect URIs: `https://localhost:7001/*`
- [ ] Create roles:
  - [ ] `investor`
  - [ ] `consultant`
  - [ ] `admin`
  - [ ] `kyc-verified`

### Test Users in Keycloak
- [ ] Create investor user: `investor@test.com`
  - [ ] Assign role: `investor`
- [ ] Create consultant user: `consultant@test.com`
  - [ ] Assign roles: `consultant`
- [ ] Create admin user: `admin@test.com`
  - [ ] Assign role: `admin`

### API Project Setup
- [ ] Create project: `dotnet new webapi -n GoalGrow.Api`
- [ ] Add to solution: `dotnet sln add GoalGrow.Api`
- [ ] Add project references (Data, Entity)
- [ ] Add NuGet packages:
  - [ ] `Microsoft.AspNetCore.Authentication.JwtBearer`
  - [ ] `Microsoft.AspNetCore.Authentication.OpenIdConnect`
  - [ ] `Swashbuckle.AspNetCore`
  - [ ] `AutoMapper.Extensions.Microsoft.DependencyInjection`
  - [ ] `FluentValidation.AspNetCore`

### API Authentication Implementation
- [ ] Configure JWT Bearer in `Program.cs`
- [ ] Add Keycloak Authority URL to `appsettings.json`
- [ ] Create authorization policies (InvestorOnly, ConsultantOnly, KycVerified)
- [ ] Test with Postman/curl
  - [ ] Get JWT token from Keycloak
  - [ ] Call protected endpoint with token
  - [ ] Verify role-based access works

---

## Phase 3: Core Services & API

### Service Layer
- [ ] Create `GoalGrow.Api/Services/Interfaces/`
- [ ] Create `GoalGrow.Api/Services/Implementations/`
- [ ] Implement `IUserService`
  - [ ] `GetOrCreateUserAsync(ClaimsPrincipal)` - Sync with Keycloak
  - [ ] `UpdateUserProfileAsync()`
  - [ ] `DeleteUserAsync()` - GDPR right to be forgotten
- [ ] Implement `IGoalService`
  - [ ] `CreateSystemGoalsAsync(userId)` - Auto-create Emergency + Investment
  - [ ] `CreateCustomGoalAsync()`
  - [ ] `UpdateGoalAsync()`
  - [ ] `DeleteGoalAsync()` - Prevent deletion of system goals
  - [ ] `DepositToGoalAsync()`
  - [ ] `WithdrawFromGoalAsync()`
- [ ] Implement `IKycService`
  - [ ] `SubmitKycDocumentsAsync()`
  - [ ] `GetKycStatusAsync()`
  - [ ] `VerifyKycAsync()` - Admin only
  - [ ] `RejectKycAsync()` - Admin only
- [ ] Implement `IPlatformFeeService`
  - [ ] `CalculateFeeAsync()`
  - [ ] `ApplyFeeAsync()`
  - [ ] `GetUserFeesAsync()`

### DTOs (Data Transfer Objects)
- [ ] Create `GoalGrow.Api/DTOs/Requests/`
  - [ ] `CreateGoalRequest.cs`
  - [ ] `DepositRequest.cs`
  - [ ] `WithdrawRequest.cs`
  - [ ] `SubmitKycRequest.cs`
  - [ ] `CreateInvestmentRequest.cs`
- [ ] Create `GoalGrow.Api/DTOs/Responses/`
  - [ ] `UserResponse.cs`
  - [ ] `GoalResponse.cs`
  - [ ] `KycStatusResponse.cs`
  - [ ] `TransactionResponse.cs`

### AutoMapper Profiles
- [ ] Create `AutoMapperProfile.cs`
  - [ ] User  UserResponse
  - [ ] Goal  GoalResponse
  - [ ] KycVerification  KycStatusResponse

### API Controllers
- [ ] `AuthController.cs`
  - [ ] `POST /api/auth/register` - Create user in GoalGrow DB
  - [ ] `GET /api/auth/me` - Get current user
- [ ] `GoalController.cs`
  - [ ] `GET /api/goals` - List user goals
  - [ ] `POST /api/goals` - Create custom goal
  - [ ] `PUT /api/goals/{id}` - Update goal
  - [ ] `DELETE /api/goals/{id}` - Delete goal (check IsSystemGoal)
  - [ ] `POST /api/goals/{id}/deposit` - Deposit to goal
  - [ ] `POST /api/goals/{id}/withdraw` - Withdraw from goal
- [ ] `KycController.cs`
  - [ ] `POST /api/kyc/submit` - Upload KYC documents
  - [ ] `GET /api/kyc/status` - Check KYC status
  - [ ] `POST /api/kyc/verify` - Admin: Verify KYC
  - [ ] `POST /api/kyc/reject` - Admin: Reject KYC
- [ ] `InvestmentController.cs` (Future)
  - [ ] `GET /api/consultants` - Marketplace
  - [ ] `POST /api/investment/create`

### Middleware
- [ ] Create `ExceptionHandlingMiddleware.cs`
- [ ] Create `RequestLoggingMiddleware.cs`
- [ ] Create `RateLimitingMiddleware.cs`

### Validation
- [ ] FluentValidation validators:
  - [ ] `CreateGoalRequestValidator.cs`
  - [ ] `DepositRequestValidator.cs`
  - [ ] `SubmitKycRequestValidator.cs`

---

## Phase 4: Blazor Web App

### Project Setup
- [ ] Create project: `dotnet new blazorserver -n GoalGrow.Web`
- [ ] Add to solution
- [ ] Add NuGet packages:
  - [ ] `Microsoft.AspNetCore.Authentication.OpenIdConnect`
  - [ ] `Radzen.Blazor` (UI components)
  - [ ] `Blazored.LocalStorage`

### Authentication
- [ ] Configure OpenID Connect in `Program.cs`
- [ ] Create `LoginDisplay.razor` component
- [ ] Test login/logout flow
- [ ] Test token refresh

### Layout & Navigation
- [ ] Update `MainLayout.razor`
- [ ] Create `NavMenu.razor` with sections:
  - [ ] Dashboard
  - [ ] Goals
  - [ ] Transactions
  - [ ] Investment (locked until KYC)
  - [ ] Profile

### Pages
- [ ] `Pages/Index.razor` - Dashboard
  - [ ] Show virtual wallet balance
  - [ ] Show goal progress cards
  - [ ] Recent transactions list
  - [ ] XP and level display
- [ ] `Pages/Goals/Goals.razor`
  - [ ] List all goals (Emergency, Investment, Custom)
  - [ ] Progress bars
  - [ ] Create new goal button
- [ ] `Pages/Goals/CreateGoal.razor`
  - [ ] Form with validation
  - [ ] Goal type selector
  - [ ] Amount and date pickers
- [ ] `Pages/Kyc/Upload.razor`
  - [ ] File upload for documents
  - [ ] Selfie capture (webcam)
  - [ ] Submit button
- [ ] `Pages/Kyc/Status.razor`
  - [ ] KYC status badge
  - [ ] Document upload checklist
  - [ ] Next steps guidance

### Components
- [ ] `Components/GoalCard.razor` - Reusable goal display
- [ ] `Components/TransactionList.razor`
- [ ] `Components/ProgressBar.razor`
- [ ] `Components/LevelBadge.razor`

### HTTP Services (API Clients)
- [ ] Create `Services/ApiClient/IGoalApiClient.cs`
- [ ] Create `Services/ApiClient/IKycApiClient.cs`
- [ ] Create `Services/ApiClient/IUserApiClient.cs`
- [ ] Inject HttpClient with JWT bearer token

---

## Phase 5: Features Implementation

### Goal Management
- [ ] Auto-create Emergency + Investment goals on registration
- [ ] Prevent deletion of system goals
- [ ] Unlock investment threshold notification
- [ ] Goal progress tracking
- [ ] Auto-save recurring deposits

### Virtual Wallet
- [ ] Deposit flow:
  - [ ] User selects goal
  - [ ] Enters amount
  - [ ] Platform fee calculated (1%, min 1)
  - [ ] Confirmation screen
  - [ ] Transaction created
  - [ ] Balance updated
- [ ] Withdrawal flow:
  - [ ] Check if goal is locked
  - [ ] Calculate fee
  - [ ] Update balance
- [ ] Transaction history page

### KYC Workflow
- [ ] Document upload to Azure Blob Storage
- [ ] Status tracking (Pending  Under Review  Verified)
- [ ] Email notification on status change
- [ ] Admin dashboard for manual verification
- [ ] Unlock "kyc-verified" role in Keycloak after approval

### Investment Unlocking
- [ ] Monitor Investment goal balance
- [ ] Send notification when threshold reached
- [ ] Show RiskProfile questionnaire
- [ ] Unlock consultant marketplace

### Gamification
- [ ] XP system:
  - [ ] Award XP on key actions
  - [ ] Level up when threshold reached
  - [ ] Display level badge
- [ ] Badge system:
  - [ ] Define badge criteria
  - [ ] Check for badge unlock on each action
  - [ ] Display earned badges
- [ ] Challenge system:
  - [ ] Create sample challenges
  - [ ] Track progress
  - [ ] Award rewards on completion

---

## Phase 6: Testing

### Unit Tests
- [ ] `UserService` tests
  - [ ] Test GetOrCreateUser creates new user
  - [ ] Test GetOrCreateUser returns existing user
- [ ] `GoalService` tests
  - [ ] Test CreateSystemGoals creates Emergency + Investment
  - [ ] Test cannot delete system goals
  - [ ] Test deposit calculates fee correctly
- [ ] `PlatformFeeService` tests
  - [ ] Test fee calculation (1%, min 1)
  - [ ] Test 300 deposit = 3 fee
  - [ ] Test 50 deposit = 1 fee (minimum)
- [ ] `KycService` tests
  - [ ] Test KYC workflow states
  - [ ] Test rejection flow

### Integration Tests
- [ ] Test API endpoints with test database
- [ ] Test JWT authentication
- [ ] Test role-based authorization
- [ ] Test CRUD operations

### E2E Tests (Playwright/Selenium)
- [ ] Test registration flow
- [ ] Test KYC upload flow
- [ ] Test goal creation
- [ ] Test deposit/withdrawal

---

## Phase 7: Security & Compliance

### Security
- [ ] HTTPS enforcement
- [ ] CORS configuration
- [ ] Rate limiting
- [ ] SQL injection prevention (EF parameterized queries)
- [ ] XSS prevention
- [ ] CSRF tokens
- [ ] Secure headers (HSTS, CSP, X-Frame-Options)
- [ ] Input validation on all endpoints
- [ ] File upload validation (size, type)

### GDPR Compliance
- [ ] Cookie consent banner
- [ ] Privacy policy page
- [ ] Terms of service page
- [ ] User data export (GDPR right to access)
- [ ] User data deletion (GDPR right to erasure)
- [ ] Data retention policy (KYC docs 5 years)

### Logging & Monitoring
- [ ] Serilog setup
- [ ] Application Insights integration (Azure)
- [ ] Error tracking (Sentry)
- [ ] Performance monitoring
- [ ] Security event logging (failed logins, KYC rejections)

---

## Phase 8: Deployment

### Azure Setup
- [ ] Create Azure account
- [ ] Create Resource Group: `rg-goalgrow-prod`
- [ ] Create SQL Server + Database
- [ ] Create App Service for API
- [ ] Create App Service for Web
- [ ] Create Azure Blob Storage for KYC docs
- [ ] Create Azure Key Vault for secrets
- [ ] Configure managed identities

### CI/CD Pipeline
- [ ] Create GitHub Actions workflow
  - [ ] Build on PR
  - [ ] Run tests
  - [ ] Deploy to staging on merge to `develop`
  - [ ] Deploy to prod on merge to `main`
- [ ] Configure environment variables in GitHub Secrets
- [ ] Set up deployment slots (staging/prod)

### Production Keycloak
- [ ] Set up managed Keycloak (Red Hat SSO or self-hosted)
- [ ] Configure production realm
- [ ] Migrate dev users (if needed)
- [ ] Set up backups

### Monitoring
- [ ] Application Insights dashboards
- [ ] Alert rules:
  - [ ] API response time > 1s
  - [ ] Error rate > 1%
  - [ ] Failed logins > 10/min
- [ ] Health check endpoints

---

## Phase 9: Go-Live Preparation

### Documentation
- [ ] API documentation (Swagger)
- [ ] User guide
- [ ] Admin guide (KYC verification)
- [ ] Deployment runbook
- [ ] Incident response playbook

### Load Testing
- [ ] Apache JMeter or k6 tests
- [ ] Test with 100 concurrent users
- [ ] Test with 1000 concurrent users
- [ ] Identify bottlenecks

### Security Audit
- [ ] Run OWASP ZAP scan
- [ ] Penetration testing (hire external firm)
- [ ] Fix critical vulnerabilities

### Legal
- [ ] Privacy policy reviewed by lawyer
- [ ] Terms of service reviewed
- [ ] GDPR compliance audit
- [ ] Financial license requirements (consult lawyer)

---

## Phase 10: Post-Launch

### Marketing
- [ ] Landing page
- [ ] Social media accounts
- [ ] Blog for SEO
- [ ] Referral program

### Monitoring
- [ ] Daily review of logs
- [ ] Weekly KPI report
- [ ] Monthly user feedback survey

### Iteration
- [ ] Collect user feedback
- [ ] Prioritize feature requests
- [ ] Plan next sprint

---

##  Progress Tracking

**Overall Progress:** 0 / 200+ tasks completed

**Phase Completion:**
- [ ] Phase 1: Foundation (0%)
- [ ] Phase 2: Authentication (0%)
- [ ] Phase 3: API (0%)
- [ ] Phase 4: Web App (0%)
- [ ] Phase 5: Features (0%)
- [ ] Phase 6: Testing (0%)
- [ ] Phase 7: Security (0%)
- [ ] Phase 8: Deployment (0%)
- [ ] Phase 9: Go-Live (0%)
- [ ] Phase 10: Post-Launch (0%)

---

##  Next Action

**Start here:**
```powershell
# 1. Run migration script
.\Migrate-Configurations.ps1 -Backup

# 2. Create new branch
git checkout -b feature/database-refactoring

# 3. Commit changes
git add .
git commit -m "refactor: reorganize EF configurations by module"
git push origin feature/database-refactoring
```

**Then proceed with checklist above!** 
