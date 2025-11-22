using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GoalGrow.API.Controllers
{
    /// <summary>
    /// Savings goals management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication
    public class GoalsController : ControllerBase
    {
        private readonly IGoalService _goalService;
        private readonly IUserService _userService;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(
            IGoalService goalService,
            IUserService userService,
            ILogger<GoalsController> logger)
        {
            _goalService = goalService;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all goals for the current user (INVESTOR ONLY)
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
        /// <param name="status">Filter by status (Active, Completed, OnHold, Cancelled)</param>
        /// <param name="type">Filter by type (Emergency, Investment, Custom)</param>
        /// <returns>Paginated list of goals</returns>
        /// <response code="200">Goals retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - only investors can view goals</response>
        [HttpGet]
        [Authorize(Roles = "investor")]
        [ProducesResponseType(typeof(ApiResponse<GoalListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyGoals(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);

                if (pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Page size must be between 1 and 100"));
                }

                var result = await _goalService.GetUserGoalsAsync(user.Id, pageNumber, pageSize, status, type);

                _logger.LogInformation("Retrieved {Count} goals for user {UserId}", result.Goals.Count, user.Id);

                return Ok(ApiResponse<GoalListResponse>.SuccessResponse(
                    result,
                    $"Retrieved {result.Goals.Count} goal(s)"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goals");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get a specific goal by ID (INVESTOR ONLY)
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Goal details</returns>
        /// <response code="200">Goal retrieved successfully</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - only investors can view goals</response>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "investor")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGoalById(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.GetGoalByIdAsync(id, user.Id);

                if (goal == null)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Goal not found"));
                }

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(
                    goal,
                    "Goal retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Create a new savings goal
        /// </summary>
        /// <param name="request">Goal creation data</param>
        /// <returns>Created goal</returns>
        /// <response code="201">Goal created successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateGoal([FromBody] CreateGoalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", errors));
                }

                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.CreateGoalAsync(user.Id, request);

                _logger.LogInformation("Goal created: {GoalId} for user {UserId}", goal.Id, user.Id);

                return CreatedAtAction(
                    nameof(GetGoalById),
                    new { id = goal.Id },
                    ApiResponse<GoalResponse>.SuccessResponse(goal, "Goal created successfully")
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid goal creation attempt: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Cannot create goal: {Message}", ex.Message);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Update an existing goal
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <param name="request">Updated goal data</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Goal updated successfully</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateGoal(Guid id, [FromBody] UpdateGoalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", errors));
                }

                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.UpdateGoalAsync(id, user.Id, request);

                _logger.LogInformation("Goal updated: {GoalId}", id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(goal, "Goal updated successfully"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Delete a goal (soft delete - marks as cancelled)
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Confirmation of deletion</returns>
        /// <response code="200">Goal deleted successfully</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteGoal(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var deleted = await _goalService.DeleteGoalAsync(id, user.Id);

                if (!deleted)
                {
                    return StatusCode(500, ApiResponse<object>.ErrorResponse("Failed to delete goal"));
                }

                _logger.LogInformation("Goal deleted: {GoalId}", id);

                return Ok(ApiResponse<object>.SuccessResponse(
                    new { GoalId = id, DeletedAt = DateTime.UtcNow },
                    "Goal deleted successfully. Funds returned to wallet if applicable."
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get detailed progress for a goal
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Progress details with milestones and recommendations</returns>
        /// <response code="200">Progress retrieved successfully</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{id:guid}/progress")]
        [ProducesResponseType(typeof(ApiResponse<GoalProgressResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetGoalProgress(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var progress = await _goalService.GetGoalProgressAsync(id, user.Id);

                return Ok(ApiResponse<GoalProgressResponse>.SuccessResponse(
                    progress,
                    "Progress retrieved successfully"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving progress for goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Contribute funds to a goal from virtual wallet
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <param name="request">Contribution data</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Contribution successful</response>
        /// <response code="400">Invalid request or insufficient funds</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{id:guid}/contribute")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ContributeToGoal(Guid id, [FromBody] GoalContributionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", errors));
                }

                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.ContributeToGoalAsync(id, user.Id, request);

                _logger.LogInformation("Contributed €{Amount} to goal {GoalId}", request.Amount, id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(
                    goal,
                    $"Successfully contributed €{request.Amount:F2} to goal"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error contributing to goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Withdraw funds from a goal back to virtual wallet
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <param name="request">Withdrawal data</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Withdrawal successful</response>
        /// <response code="400">Invalid request or insufficient funds</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{id:guid}/withdraw")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> WithdrawFromGoal(Guid id, [FromBody] GoalWithdrawalRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<object>.ErrorResponse("Invalid request data", errors));
                }

                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.WithdrawFromGoalAsync(id, user.Id, request);

                _logger.LogInformation("Withdrawn €{Amount} from goal {GoalId}", request.Amount, id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(
                    goal,
                    $"Successfully withdrawn €{request.Amount:F2} from goal"
                ));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing from goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Mark a goal as completed manually
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Goal marked as completed</response>
        /// <response code="400">Invalid operation</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CompleteGoal(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.CompleteGoalAsync(id, user.Id);

                _logger.LogInformation("Goal completed manually: {GoalId}", id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(goal, "Goal marked as completed"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Pause a goal (stops auto-save)
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Goal paused</response>
        /// <response code="400">Invalid operation</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{id:guid}/pause")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PauseGoal(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.PauseGoalAsync(id, user.Id);

                _logger.LogInformation("Goal paused: {GoalId}", id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(goal, "Goal paused"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pausing goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Resume a paused goal
        /// </summary>
        /// <param name="id">Goal ID</param>
        /// <returns>Updated goal</returns>
        /// <response code="200">Goal resumed</response>
        /// <response code="400">Invalid operation</response>
        /// <response code="404">Goal not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("{id:guid}/resume")]
        [ProducesResponseType(typeof(ApiResponse<GoalResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ResumeGoal(Guid id)
        {
            try
            {
                var user = await _userService.GetOrCreateUserAsync(User);
                var goal = await _goalService.ResumeGoalAsync(id, user.Id);

                _logger.LogInformation("Goal resumed: {GoalId}", id);

                return Ok(ApiResponse<GoalResponse>.SuccessResponse(goal, "Goal resumed"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resuming goal {GoalId}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        /// <summary>
        /// Get platform-wide goal statistics (Admin only)
        /// </summary>
        /// <returns>Goal statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - requires admin role</response>
        [HttpGet("stats")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(ApiResponse<GoalStatsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetGoalStats()
        {
            try
            {
                var stats = await _goalService.GetGoalStatsAsync();

                _logger.LogInformation("Admin retrieved goal statistics");

                return Ok(ApiResponse<GoalStatsResponse>.SuccessResponse(
                    stats,
                    "Platform goal statistics retrieved successfully"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goal statistics");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }
    }
}
