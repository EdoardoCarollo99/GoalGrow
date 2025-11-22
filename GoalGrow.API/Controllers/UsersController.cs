using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Extensions;
using GoalGrow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoalGrow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get current authenticated user's profile
        /// </summary>
        /// <returns>User profile data</returns>
        /// <response code="200">User profile retrieved successfully</response>
        /// <response code="401">Unauthorized - invalid or missing token</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userResponse = await _userService.GetCurrentUserAsync(User);
                
                _logger.LogInformation("User profile retrieved: {UserId}", userResponse.Id);
                
                return Ok(ApiResponse<UserResponse>.SuccessResponse(
                    userResponse, 
                    "User profile retrieved successfully"
                ));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid authentication token"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Update current user's profile
        /// </summary>
        /// <param name="request">Updated profile data</param>
        /// <returns>Updated user profile</returns>
        /// <response code="200">Profile updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized - invalid or missing token</response>
        [HttpPut("me")]
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                // Get current user from Keycloak claims
                var user = await _userService.GetOrCreateUserAsync(User);
                
                var updatedUser = await _userService.UpdateUserProfileAsync(user.Id, request);
                
                _logger.LogInformation("User profile updated: {UserId}", user.Id);
                
                return Ok(ApiResponse<UserResponse>.SuccessResponse(
                    updatedUser, 
                    "Profile updated successfully"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("User not found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid authentication token"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get user by ID (Admin only for now)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User profile data</returns>
        /// <response code="200">User found</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{userId:guid}")]
        [Authorize(Roles = "admin")] // Only admins can view other users
        [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning("User not found: {UserId}", userId);
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
                }

                // Create a temporary ClaimsPrincipal for mapping (not ideal, but works for now)
                // TODO: Refactor to have a separate mapping method
                var userResponse = new UserResponse
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
                
                return Ok(ApiResponse<UserResponse>.SuccessResponse(
                    userResponse, 
                    "User retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get current user's wallet information (Investor only)
        /// </summary>
        /// <returns>Wallet details including balance, deposits, investments</returns>
        /// <response code="200">Wallet information retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - not an investor user</response>
        [HttpGet("me/wallet")]
        [Authorize(Roles = "investor")]
        [ProducesResponseType(typeof(ApiResponse<WalletResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyWallet()
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var wallet = await _userService.GetUserWalletAsync(user.Id);

                _logger.LogInformation("Wallet retrieved for user {UserId}", user.Id);

                return Ok(ApiResponse<WalletResponse>.SuccessResponse(
                    wallet,
                    "Wallet information retrieved successfully"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("User not found: {Message}", ex.Message);
                return NotFound(ApiResponse<object>.ErrorResponse("User not found or not an investor"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving wallet");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get current user's accounts (bank accounts, payment methods)
        /// </summary>
        /// <returns>List of user accounts</returns>
        /// <response code="200">Accounts retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("me/accounts")]
        [ProducesResponseType(typeof(ApiResponse<List<AccountSummaryResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyAccounts()
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var accounts = await _userService.GetUserAccountsAsync(user.Id);

                _logger.LogInformation("Accounts retrieved for user {UserId}, Count: {Count}", user.Id, accounts.Count);

                return Ok(ApiResponse<List<AccountSummaryResponse>>.SuccessResponse(
                    accounts,
                    $"{accounts.Count} account(s) retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user accounts");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Delete current user account (GDPR right to be forgotten)
        /// </summary>
        /// <param name="reason">Optional reason for deletion</param>
        /// <returns>Confirmation of deletion</returns>
        /// <response code="200">Account deleted successfully</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("me")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMyAccount([FromQuery] string? reason = null)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                
                _logger.LogWarning("User deletion requested by {UserId}. Reason: {Reason}", user.Id, reason ?? "Not specified");

                var deleted = await _userService.DeleteUserAccountAsync(user.Id, reason);

                if (!deleted)
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete account"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { UserId = user.Id, DeletedAt = DateTime.UtcNow },
                    "Account deleted successfully. Your data has been anonymized."
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user account");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }
    }
}
