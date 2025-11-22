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
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login con username e password
        /// </summary>
        /// <param name="request">Credenziali utente</param>
        /// <returns>Token JWT e informazioni utente</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Login failed for user {Username}: {Message}", request.Username, ex.Message);
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid username or password"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Login error for user {Username}", request.Username);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Authentication service unavailable"));
            }
        }

        /// <summary>
        /// Rinnova il token JWT usando il refresh token
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>Nuovo token JWT</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Token refreshed"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid or expired refresh token"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Token refresh error");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Authentication service unavailable"));
            }
        }

        /// <summary>
        /// Logout (invalida il refresh token)
        /// </summary>
        /// <param name="request">Refresh token da invalidare</param>
        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            try
            {
                await _authService.LogoutAsync(request.RefreshToken);
                return Ok(ApiResponse<object>.SuccessResponse("Logout successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout error");
                // Anche se fallisce, considera il logout riuscito lato client
                return Ok(ApiResponse<object>.SuccessResponse("Logout completed"));
            }
        }

        /// <summary>
        /// Verifica se il token corrente è valido
        /// </summary>
        [HttpGet("verify")]
        [Authorize]
        public IActionResult Verify()
        {
            // Usa extension methods per gestire claim mappati
            var userId = User.GetUserId();
            var username = User.Identity?.Name;
            var email = User.GetEmail();

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                UserId = userId,
                Username = username,
                Email = email,
                IsAuthenticated = true
            }, "Token is valid"));
        }
    }

    /// <summary>
    /// Richiesta per refresh/logout
    /// </summary>
    public class RefreshTokenRequest
    {
        public required string RefreshToken { get; set; }
    }
}
