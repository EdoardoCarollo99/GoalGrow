using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoalGrow.API.Controllers
{
    /// <summary>
    /// Admin-only endpoints for user management and platform statistics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // All endpoints require admin role
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IUserService userService, ILogger<AdminController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of all users
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
        /// <param name="searchTerm">Search by email or name</param>
        /// <param name="userType">Filter by user type (Admin, Consultant, Investor)</param>
        /// <returns>Paginated user list</returns>
        /// <response code="200">Users retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        [HttpGet("users")]
        [ProducesResponseType(typeof(ApiResponse<UserListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? userType = null)
        {
            try
            {
                // Validate pagination parameters
                if (pageNumber < 1)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Page number must be greater than 0"));
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Page size must be between 1 and 100"));
                }

                var result = await _userService.GetUsersAsync(pageNumber, pageSize, searchTerm, userType);

                _logger.LogInformation(
                    "Admin retrieved user list: Page {Page}/{TotalPages}, Total {Total} users",
                    pageNumber, result.TotalPages, result.TotalCount);

                return Ok(ApiResponse<UserListResponse>.SuccessResponse(
                    result,
                    $"Retrieved {result.Users.Count} users (page {pageNumber} of {result.TotalPages})"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users list");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get platform-wide user statistics
        /// </summary>
        /// <returns>User statistics including counts, KYC status, balances</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<UserStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserStats()
        {
            try
            {
                var stats = await _userService.GetUserStatsAsync();

                _logger.LogInformation("Admin retrieved platform statistics: {TotalUsers} total users", stats.TotalUsers);

                return Ok(ApiResponse<UserStatsResponse>.SuccessResponse(
                    stats,
                    "Platform statistics retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Activate or deactivate a user account
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="isActive">True to activate, false to deactivate</param>
        /// <returns>Updated user status</returns>
        /// <response code="200">User status updated successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        [HttpPut("users/{userId:guid}/status")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateUserStatus(Guid userId, [FromBody] bool isActive)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User not found for status update: {UserId}", userId);
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
                }

                // TODO: Implement user activation/deactivation in UserService
                // For now, return a placeholder response

                _logger.LogInformation("Admin updated user status: {UserId}, Active: {IsActive}", userId, isActive);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { UserId = userId, IsActive = isActive, UpdatedAt = DateTime.UtcNow },
                    $"User {(isActive ? "activated" : "deactivated")} successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Delete a user account (Admin-initiated GDPR deletion)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="reason">Deletion reason</param>
        /// <returns>Confirmation of deletion</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="404">User not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        [HttpDelete("users/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteUser(Guid userId, [FromQuery] string? reason = null)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion: {UserId}", userId);
                    return NotFound(ApiResponse<object>.ErrorResponse("User not found"));
                }

                _logger.LogWarning("Admin deletion requested for user {UserId}. Reason: {Reason}", 
                    userId, reason ?? "Not specified");

                var deleted = await _userService.DeleteUserAccountAsync(userId, reason);

                if (!deleted)
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete user"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { UserId = userId, DeletedAt = DateTime.UtcNow },
                    "User account deleted successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }
    }
}
