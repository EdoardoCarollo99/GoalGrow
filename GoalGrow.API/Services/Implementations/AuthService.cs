using GoalGrow.API.DTOs.Requests;
using GoalGrow.API.DTOs.Responses;
using GoalGrow.API.Extensions;
using GoalGrow.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GoalGrow.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IConfiguration configuration, 
            IHttpClientFactory httpClientFactory,
            ILogger<AuthService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var keycloakSettings = _configuration.GetSection("Keycloak");
            var authority = keycloakSettings["Authority"];
            var clientId = keycloakSettings["ClientId"];
            var clientSecret = keycloakSettings["ClientSecret"];
            
            var tokenEndpoint = $"{authority}/protocol/openid-connect/token";

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "username", request.Username },
                { "password", request.Password },
                { "scope", "openid profile email" }
            };

            _logger.LogInformation("Login attempt for user: {Username}", request.Username);

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(formData));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Login failed for user {Username}: {StatusCode} - {Response}", 
                        request.Username, response.StatusCode, content);
                    
                    throw new UnauthorizedAccessException("Invalid username or password");
                }

                var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(content);

                if (tokenResponse == null)
                    throw new InvalidOperationException("Failed to parse Keycloak response");

                // Decodifica il token per estrarre le informazioni utente
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);

                var loginResponse = new LoginResponse
                {
                    AccessToken = tokenResponse.AccessToken,
                    TokenType = tokenResponse.TokenType ?? "Bearer",
                    ExpiresIn = tokenResponse.ExpiresIn,
                    RefreshToken = tokenResponse.RefreshToken ?? string.Empty,
                    Scope = tokenResponse.Scope,
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
                    Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? string.Empty,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    FullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                    Roles = ExtractRoles(jwtToken)
                };

                _logger.LogInformation("Login successful for user: {Username} (ID: {UserId})", 
                    loginResponse.Username, loginResponse.UserId);

                return loginResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during login for user {Username}", request.Username);
                throw new InvalidOperationException("Unable to connect to authentication server", ex);
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var keycloakSettings = _configuration.GetSection("Keycloak");
            var authority = keycloakSettings["Authority"];
            var clientId = keycloakSettings["ClientId"];
            var clientSecret = keycloakSettings["ClientSecret"];
            
            var tokenEndpoint = $"{authority}/protocol/openid-connect/token";

            var formData = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "refresh_token", refreshToken }
            };

            _logger.LogInformation("Refreshing token");

            try
            {
                var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(formData));
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Token refresh failed: {StatusCode}", response.StatusCode);
                    throw new UnauthorizedAccessException("Invalid or expired refresh token");
                }

                var tokenResponse = JsonSerializer.Deserialize<KeycloakTokenResponse>(content);

                if (tokenResponse == null)
                    throw new InvalidOperationException("Failed to parse Keycloak response");

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenResponse.AccessToken);

                return new LoginResponse
                {
                    AccessToken = tokenResponse.AccessToken,
                    TokenType = tokenResponse.TokenType ?? "Bearer",
                    ExpiresIn = tokenResponse.ExpiresIn,
                    RefreshToken = tokenResponse.RefreshToken ?? refreshToken,
                    Scope = tokenResponse.Scope,
                    UserId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
                    Username = jwtToken.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ?? string.Empty,
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value,
                    FullName = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                    Roles = ExtractRoles(jwtToken)
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during token refresh");
                throw new InvalidOperationException("Unable to connect to authentication server", ex);
            }
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var keycloakSettings = _configuration.GetSection("Keycloak");
            var authority = keycloakSettings["Authority"];
            var clientId = keycloakSettings["ClientId"];
            var clientSecret = keycloakSettings["ClientSecret"];
            
            var logoutEndpoint = $"{authority}/protocol/openid-connect/logout";

            var formData = new Dictionary<string, string>
            {
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "refresh_token", refreshToken }
            };

            _logger.LogInformation("Logging out user");

            try
            {
                var response = await _httpClient.PostAsync(logoutEndpoint, new FormUrlEncodedContent(formData));
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Logout failed: {StatusCode}", response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error during logout");
                // Non lanciare eccezione, logout è best-effort
            }
        }

        private static IEnumerable<string> ExtractRoles(JwtSecurityToken token)
        {
            var realmAccessClaim = token.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            
            if (string.IsNullOrEmpty(realmAccessClaim))
                return [];

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

            return [];
        }

        // DTO interno per Keycloak response - con mapping snake_case
        private class KeycloakTokenResponse
        {
            [JsonPropertyName("access_token")]
            public required string AccessToken { get; set; }
            
            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
            
            [JsonPropertyName("refresh_expires_in")]
            public int RefreshExpiresIn { get; set; }
            
            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }
            
            [JsonPropertyName("token_type")]
            public string? TokenType { get; set; }
            
            [JsonPropertyName("scope")]
            public string? Scope { get; set; }
        }
    }
}
