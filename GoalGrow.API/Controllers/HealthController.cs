using GoalGrow.API.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoalGrow.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint (no auth required)
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            _logger.LogInformation("Health check requested");
            
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0"
            }, "API is running"));
        }

        /// <summary>
        /// Authenticated endpoint test
        /// </summary>
        [HttpGet("secure")]
        [Authorize]
        public IActionResult GetSecure()
        {
            var userId = User.FindFirst("sub")?.Value;
            var email = User.FindFirst("email")?.Value;
            
            _logger.LogInformation("Secure endpoint accessed by {Email}", email);
            
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                Message = "You are authenticated!",
                UserId = userId,
                Email = email,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }));
        }
    }
}
