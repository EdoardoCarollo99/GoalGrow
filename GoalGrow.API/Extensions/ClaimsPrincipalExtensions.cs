using System.Security.Claims;
using System.Text.Json;

namespace GoalGrow.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Ottiene il claim sub (user ID) da JWT, supportando sia nomi standard che Microsoft
        /// </summary>
        public static string? GetUserId(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("sub")?.Value
                ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }

        /// <summary>
        /// Ottiene il claim email da JWT, supportando sia nomi standard che Microsoft
        /// </summary>
        public static string? GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("email")?.Value
                ?? principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
        }

        /// <summary>
        /// Ottiene i ruoli Keycloak dal claim realm_access
        /// </summary>
        public static IEnumerable<string> GetKeycloakRoles(this ClaimsPrincipal principal)
        {
            var realmAccessClaim = principal.FindFirst("realm_access")?.Value;
            
            if (string.IsNullOrEmpty(realmAccessClaim))
                return Enumerable.Empty<string>();

            try
            {
                var realmAccess = JsonDocument.Parse(realmAccessClaim);
                if (realmAccess.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    return rolesElement.EnumerateArray()
                        .Select(r => r.GetString())
                        .Where(r => !string.IsNullOrEmpty(r))!;
                }
            }
            catch (JsonException)
            {
                // Log error se necessario
            }

            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Verifica se l'utente ha un ruolo Keycloak specifico
        /// </summary>
        public static bool HasKeycloakRole(this ClaimsPrincipal principal, string role)
        {
            return principal.GetKeycloakRoles().Contains(role, StringComparer.OrdinalIgnoreCase);
        }
    }
}
