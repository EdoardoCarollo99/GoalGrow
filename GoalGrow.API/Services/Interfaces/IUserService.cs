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
    }
}
