using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Extensions;
using GoalGrow.API.Services.Interfaces;
using GoalGrow.Data;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Models;
using GoalGrow.Entity.Super;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GoalGrow.API.Services.Implementations
{
    /// <summary>
    /// Service for user management and Keycloak synchronization
    /// </summary>
    public class UserService : IUserService
    {
        private readonly GoalGrowDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(GoalGrowDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets or creates a user from JWT claims (Keycloak sync)
        /// This is the main entry point for synchronizing Keycloak users with the database
        /// </summary>
        public async Task<User> GetOrCreateUserAsync(ClaimsPrincipal claims)
        {
            // Log all claims for debugging
            var allClaims = claims.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
            _logger.LogDebug("Received claims: {Claims}", string.Join(", ", allClaims));

            // Extract claims from JWT token
            // Try multiple claim names for Subject ID
            var keycloakSubjectId = claims.FindFirst("sub")?.Value
                ?? claims.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? claims.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            if (string.IsNullOrEmpty(keycloakSubjectId))
            {
                _logger.LogError("Missing subject claim in token. Available claims: {Claims}", string.Join(", ", allClaims));
                throw new UnauthorizedAccessException($"Missing 'sub' claim in token. Available claims: {allClaims.Count}");
            }

            var email = claims.GetEmail();
            
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogError("Missing email claim in token. Available claims: {Claims}", string.Join(", ", allClaims));
                throw new UnauthorizedAccessException($"Missing email claim in token. Available claims: {allClaims.Count}");
            }

            var firstName = claims.FindFirst("given_name")?.Value 
                ?? claims.FindFirst(ClaimTypes.GivenName)?.Value 
                ?? string.Empty;
                
            var lastName = claims.FindFirst("family_name")?.Value 
                ?? claims.FindFirst(ClaimTypes.Surname)?.Value 
                ?? string.Empty;
                
            var username = claims.FindFirst("preferred_username")?.Value 
                ?? claims.FindFirst(ClaimTypes.Name)?.Value 
                ?? email;

            _logger.LogInformation("Syncing user with Keycloak SubjectId: {SubjectId}, Email: {Email}", 
                keycloakSubjectId, email);

            // Try to find user by Keycloak SubjectId first (most reliable)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.KeycloakSubjectId == keycloakSubjectId);

            if (user != null)
            {
                _logger.LogInformation("User found by KeycloakSubjectId: {UserId}", user.Id);
                return user;
            }

            // If not found by SubjectId, try by email (for existing users from seed)
            user = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == email);

            if (user != null)
            {
                _logger.LogInformation("User found by email, updating KeycloakSubjectId: {UserId}", user.Id);
                
                // Update the KeycloakSubjectId for existing user
                user.KeycloakSubjectId = keycloakSubjectId;
                await _context.SaveChangesAsync();
                
                return user;
            }

            // User doesn't exist - create new one based on Keycloak roles
            _logger.LogInformation("Creating new user from Keycloak claims");
            user = CreateUserFromClaims(claims, keycloakSubjectId, email, firstName, lastName);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("New user created: {UserId}, Type: {UserType}", user.Id, user.UserType);

            return user;
        }

        /// <summary>
        /// Gets the current authenticated user's profile
        /// </summary>
        public async Task<UserResponse> GetCurrentUserAsync(ClaimsPrincipal claims)
        {
            var user = await GetOrCreateUserAsync(claims);
            return MapToUserResponse(user);
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        /// <summary>
        /// Gets a user by their Keycloak Subject ID
        /// </summary>
        public async Task<User?> GetUserByKeycloakSubjectIdAsync(string keycloakSubjectId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.KeycloakSubjectId == keycloakSubjectId);
        }

        /// <summary>
        /// Updates user profile information
        /// </summary>
        public async Task<UserResponse> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found");

            // Update properties
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;

            await _context.SaveChangesAsync();

            _logger.LogInformation("User profile updated: {UserId}", userId);

            return MapToUserResponse(user);
        }

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.EmailAddress == email);
        }

        #region Private Helper Methods

        /// <summary>
        /// Creates a new user entity based on Keycloak claims and roles
        /// </summary>
        private User CreateUserFromClaims(
            ClaimsPrincipal claims, 
            string keycloakSubjectId, 
            string email, 
            string firstName, 
            string lastName)
        {
            // Extract phone from claims if available
            var phoneNumber = claims.FindFirst("phone_number")?.Value ?? string.Empty;

            // Determine user type based on Keycloak roles
            if (claims.IsInRole("admin"))
            {
                _logger.LogInformation("Creating AdminUser for {Email}", email);
                return new AdminUser(
                    firstName: firstName,
                    lastName: lastName,
                    phoneNumber: phoneNumber,
                    emailAddress: email,
                    role: "Admin"
                )
                {
                    KeycloakSubjectId = keycloakSubjectId,
                    IsSuperAdmin = claims.IsInRole("super-admin")
                };
            }

            if (claims.IsInRole("consultant"))
            {
                _logger.LogInformation("Creating ConsultantUser for {Email}", email);
                return new ConsultantUser(
                    firstName: firstName,
                    lastName: lastName,
                    phoneNumber: phoneNumber,
                    emailAddress: email,
                    licenseNumber: GenerateTemporaryLicenseNumber()
                )
                {
                    KeycloakSubjectId = keycloakSubjectId,
                    IsActive = true
                };
            }

            // Default: create as InvestorUser
            _logger.LogInformation("Creating InvestorUser for {Email}", email);
            return new InversotorUser(
                firstName: firstName,
                lastName: lastName,
                phoneNumber: phoneNumber,
                emailAddress: email,
                fiscalCode: GenerateTemporaryFiscalCode(),
                birthDate: DateTime.UtcNow.AddYears(-25) // Placeholder
            )
            {
                KeycloakSubjectId = keycloakSubjectId
            };
        }

        /// <summary>
        /// Maps User entity to UserResponse DTO
        /// </summary>
        private UserResponse MapToUserResponse(User user)
        {
            if (user is InversotorUser investor)
            {
                return new InvestorUserResponse
                {
                    Id = investor.Id,
                    FirstName = investor.FirstName,
                    LastName = investor.LastName,
                    EmailAddress = investor.EmailAddress,
                    PhoneNumber = investor.PhoneNumber,
                    UserType = "Investor",
                    KeycloakSubjectId = investor.KeycloakSubjectId ?? string.Empty,
                    CreatedAt = DateTime.UtcNow, // TODO: Add CreatedAt to User entity
                    FiscalCode = investor.FiscalCode,
                    BirthDate = investor.BirthDate,
                    VirtualWalletBalance = investor.VirtualWalletBalance,
                    TotalDeposited = investor.TotalDeposited,
                    TotalWithdrawn = investor.TotalWithdrawn,
                    TotalInvested = investor.TotalInvested,
                    HasKycVerification = investor.KycVerification != null,
                    KycStatus = investor.KycVerification?.Status.ToString()
                };
            }

            if (user is ConsultantUser consultant)
            {
                return new ConsultantUserResponse
                {
                    Id = consultant.Id,
                    FirstName = consultant.FirstName,
                    LastName = consultant.LastName,
                    EmailAddress = consultant.EmailAddress,
                    PhoneNumber = consultant.PhoneNumber,
                    UserType = "Consultant",
                    KeycloakSubjectId = consultant.KeycloakSubjectId ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    LicenseNumber = consultant.LicenseNumber,
                    Specialization = consultant.Specialization,
                    CommissionRate = consultant.CommissionRate,
                    YearsOfExperience = consultant.YearsOfExperience,
                    AverageRating = consultant.AverageRating,
                    TotalReviews = consultant.TotalReviews
                };
            }

            if (user is AdminUser admin)
            {
                // AdminUser can use base UserResponse
                return new UserResponse
                {
                    Id = admin.Id,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    EmailAddress = admin.EmailAddress,
                    PhoneNumber = admin.PhoneNumber,
                    UserType = "Admin",
                    KeycloakSubjectId = admin.KeycloakSubjectId ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };
            }

            // Fallback to base UserResponse
            return new UserResponse
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                PhoneNumber = user.PhoneNumber,
                UserType = user.UserType.ToString(),
                KeycloakSubjectId = user.KeycloakSubjectId ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Generates a temporary license number for new consultants
        /// TODO: Replace with proper license number input from user
        /// </summary>
        private string GenerateTemporaryLicenseNumber()
        {
            return $"TEMP-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        /// <summary>
        /// Generates a temporary fiscal code for new investors
        /// TODO: Replace with proper fiscal code input from user
        /// </summary>
        private string GenerateTemporaryFiscalCode()
        {
            return $"TEMP-{Guid.NewGuid().ToString("N")[..16].ToUpper()}";
        }

        #endregion

        #region Wallet & Accounts

        /// <summary>
        /// Gets wallet information for the current investor user
        /// </summary>
        public async Task<WalletResponse> GetUserWalletAsync(Guid userId)
        {
            var user = await _context.Users
                .OfType<InversotorUser>()
                .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new KeyNotFoundException($"Investor user with ID {userId} not found");

            // Get last transaction date
            var lastTransaction = await _context.Transactions
                .Where(t => t.Account != null && t.Account.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .FirstOrDefaultAsync();

            // Count total transactions
            var transactionCount = await _context.Transactions
                .CountAsync(t => t.Account != null && t.Account.UserId == userId);

            // Calculate total profit from investments
            // Since CurrentValue is [NotMapped], we need to calculate it based on available data
            // For now, use SellAmount if sold, otherwise use TotalAmount as approximation
            // TODO: Implement real-time product price lookup for active investments
            var investments = await _context.Investments
                .Where(i => i.UserId == userId)
                .Select(i => new
                {
                    i.Status,
                    i.TotalAmount,
                    i.SellAmount,
                    i.PurchasePrice,
                    i.Quantity
                })
                .ToListAsync();

            // Calculate profit/loss
            // For sold investments: SellAmount - TotalAmount
            // For active investments: Use TotalAmount as current value (simplified)
            decimal totalProfit = 0;
            
            if (investments.Any())
            {
                var soldInvestments = investments
                    .Where(i => i.Status == InvestmentStatus.Sold && i.SellAmount.HasValue)
                    .ToList();
                
                if (soldInvestments.Any())
                {
                    totalProfit = soldInvestments.Sum(i => i.SellAmount!.Value - i.TotalAmount);
                }
            }

            // For active investments, profit is 0 (we'd need real-time prices)
            // TODO: Integrate with real-time market data API

            return new WalletResponse
            {
                UserId = user.Id,
                CurrentBalance = user.VirtualWalletBalance,
                TotalDeposited = user.TotalDeposited,
                TotalWithdrawn = user.TotalWithdrawn,
                TotalInvested = user.TotalInvested,
                TotalProfit = totalProfit,
                AvailableForInvestment = user.VirtualWalletBalance,
                LastTransactionDate = lastTransaction?.TransactionDate ?? DateTime.UtcNow,
                TransactionCount = transactionCount
            };
        }

        /// <summary>
        /// Gets all accounts (bank accounts, payment methods) for a user
        /// </summary>
        public async Task<List<AccountSummaryResponse>> GetUserAccountsAsync(Guid userId)
        {
            var accounts = await _context.Accounts
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Status == AccountStatus.Active)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();

            return accounts.Select(account => new AccountSummaryResponse
            {
                AccountId = account.Id,
                AccountType = account.Type.ToString(),
                AccountName = account.AccountName,
                AccountNumber = MaskAccountNumber(account.AccountNumber),
                Balance = account.Balance,
                Currency = account.Currency,
                Status = account.Status.ToString(),
                IsActive = account.Status == AccountStatus.Active,
                IsPrimary = false, // TODO: Add IsPrimary field to Account entity
                CreatedAt = account.CreatedAt,
                LastUsedAt = null // TODO: Track last transaction date
            }).ToList();
        }

        #endregion

        #region Admin Operations

        /// <summary>
        /// Gets paginated list of all users (Admin only)
        /// </summary>
        public async Task<UserListResponse> GetUsersAsync(
            int pageNumber, 
            int pageSize, 
            string? searchTerm = null, 
            string? userType = null)
        {
            var query = _context.Users.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower();
                query = query.Where(u => 
                    u.EmailAddress.ToLower().Contains(search) ||
                    u.FirstName.ToLower().Contains(search) ||
                    u.LastName.ToLower().Contains(search));
            }

            // Apply user type filter
            if (!string.IsNullOrWhiteSpace(userType))
            {
                if (Enum.TryParse<UserType>(userType, ignoreCase: true, out var parsedUserType))
                {
                    query = query.Where(u => u.UserType == parsedUserType);
                }
            }

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var users = await query
                .OrderByDescending(u => u.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to response DTOs
            var userResponses = users.Select(MapToUserResponse).ToList();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return new UserListResponse
            {
                Users = userResponses,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
        }

        /// <summary>
        /// Gets platform-wide user statistics (Admin only)
        /// </summary>
        public async Task<UserStatsResponse> GetUserStatsAsync()
        {
            var totalUsers = await _context.Users.CountAsync();
            
            var investorCount = await _context.Users.OfType<InversotorUser>().CountAsync();
            var consultantCount = await _context.Users.OfType<ConsultantUser>().CountAsync();
            var adminCount = await _context.Users.OfType<AdminUser>().CountAsync();

            var kycVerified = await _context.KycVerifications
                .Where(k => k.Status == KycStatus.Verified)
                .CountAsync();

            var kycPending = await _context.KycVerifications
                .Where(k => k.Status == KycStatus.Pending)
                .CountAsync();

            var totalPlatformBalance = await _context.Users
                .OfType<InversotorUser>()
                .SumAsync(u => u.VirtualWalletBalance);

            var totalInvestments = await _context.Users
                .OfType<InversotorUser>()
                .SumAsync(u => u.TotalInvested);

            return new UserStatsResponse
            {
                TotalUsers = totalUsers,
                ActiveUsers = totalUsers, // TODO: Add IsActive field
                InactiveUsers = 0, // TODO: Add IsActive field
                InvestorCount = investorCount,
                ConsultantCount = consultantCount,
                AdminCount = adminCount,
                KycVerifiedUsers = kycVerified,
                KycPendingUsers = kycPending,
                TotalPlatformBalance = totalPlatformBalance,
                TotalInvestments = totalInvestments,
                LastUpdated = DateTime.UtcNow
            };
        }

        #endregion

        #region GDPR Compliance

        /// <summary>
        /// Soft deletes a user account (GDPR right to be forgotten)
        /// This anonymizes user data while preserving transaction integrity
        /// </summary>
        public async Task<bool> DeleteUserAccountAsync(Guid userId, string? reason = null)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException($"User with ID {userId} not found");

            _logger.LogWarning("GDPR deletion requested for user {UserId}. Reason: {Reason}", userId, reason ?? "Not specified");

            // Anonymize user data instead of hard delete (for regulatory compliance)
            user.FirstName = "DELETED";
            user.LastName = "USER";
            user.EmailAddress = $"deleted_{userId:N}@goalgrow.deleted";
            user.PhoneNumber = string.Empty;
            user.KeycloakSubjectId = null;

            // Mark as deleted (TODO: Add IsDeleted and DeletedAt fields to User entity)
            // For now, we just anonymize the data

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} data anonymized successfully", userId);

            return true;
        }

        #endregion

        #region Helper Methods (continued from existing code)

        /// <summary>
        /// Masks account number for security (shows only last 4 digits)
        /// </summary>
        private string MaskAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 4)
                return "****";

            var lastFour = accountNumber[^4..];
            return $"****{lastFour}";
        }

        #endregion
    }
}
