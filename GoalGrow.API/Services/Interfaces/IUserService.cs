using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.Entity.Super;
using System.Security.Claims;

namespace GoalGrow.API.Services.Interfaces
{
    /// <summary>
    /// Service for user management and Keycloak synchronization
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets or creates a user from JWT claims (Keycloak sync)
        /// </summary>
        /// <param name="claims">ClaimsPrincipal from JWT token</param>
        /// <returns>User entity synchronized with database</returns>
        Task<User> GetOrCreateUserAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Gets the current authenticated user's profile
        /// </summary>
        /// <param name="claims">ClaimsPrincipal from JWT token</param>
        /// <returns>User profile data</returns>
        Task<UserResponse> GetCurrentUserAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User entity or null if not found</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Gets a user by their Keycloak Subject ID
        /// </summary>
        /// <param name="keycloakSubjectId">Keycloak sub claim</param>
        /// <returns>User entity or null if not found</returns>
        Task<User?> GetUserByKeycloakSubjectIdAsync(string keycloakSubjectId);

        /// <summary>
        /// Updates user profile information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="request">Updated profile data</param>
        /// <returns>Updated user profile</returns>
        Task<UserResponse> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request);

        /// <summary>
        /// Checks if a user exists by email
        /// </summary>
        /// <param name="email">Email address</param>
        /// <returns>True if user exists</returns>
        Task<bool> UserExistsByEmailAsync(string email);

        // NEW METHODS

        /// <summary>
        /// Gets wallet information for the current investor user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Wallet details</returns>
        Task<WalletResponse> GetUserWalletAsync(Guid userId);

        /// <summary>
        /// Gets all accounts (bank accounts, payment methods) for a user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user accounts</returns>
        Task<List<AccountSummaryResponse>> GetUserAccountsAsync(Guid userId);

        /// <summary>
        /// Gets paginated list of all users (Admin only)
        /// </summary>
        /// <param name="pageNumber">Page number (1-based)</param>
        /// <param name="pageSize">Items per page</param>
        /// <param name="searchTerm">Optional search term (email, name)</param>
        /// <param name="userType">Optional filter by user type</param>
        /// <returns>Paginated user list</returns>
        Task<UserListResponse> GetUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, string? userType = null);

        /// <summary>
        /// Gets platform-wide user statistics (Admin only)
        /// </summary>
        /// <returns>User statistics</returns>
        Task<UserStatsResponse> GetUserStatsAsync();

        /// <summary>
        /// Soft deletes a user account (GDPR right to be forgotten)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="reason">Deletion reason</param>
        /// <returns>True if successful</returns>
        Task<bool> DeleteUserAccountAsync(Guid userId, string? reason = null);
    }
}
