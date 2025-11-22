using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Extensions;
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
            // Usa extension methods per ottenere claim in modo robusto
            var userId = User.GetUserId();
            var email = User.GetEmail();
            var username = User.Identity?.Name;
            var name = User.FindFirst("name")?.Value;
            
            _logger.LogInformation("Secure endpoint accessed by {Email} (ID: {UserId})", email, userId);
            
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                Message = "You are authenticated!",
                UserId = userId,
                Email = email,
                Username = username,
                FullName = name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }));
        }
    }
}
