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
    }
}
