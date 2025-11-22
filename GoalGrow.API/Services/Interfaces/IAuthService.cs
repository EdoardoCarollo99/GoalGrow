using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;

namespace GoalGrow.API.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Effettua login tramite Keycloak e restituisce il token JWT
        /// </summary>
        Task<LoginResponse> LoginAsync(LoginRequest request);

        /// <summary>
        /// Rinnova il token usando il refresh token
        /// </summary>
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Logout (invalida il refresh token su Keycloak)
        /// </summary>
        Task LogoutAsync(string refreshToken);
    }
}
