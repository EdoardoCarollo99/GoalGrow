namespace GoalGrow.API.DTOs.Responses
{
    /// <summary>
    /// Risposta di login con token JWT
    /// </summary>
    public class LoginResponse
    {
        public required string AccessToken { get; set; }
        public required string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public required string RefreshToken { get; set; }
        public string? Scope { get; set; }

        // Informazioni utente estratte dal token
        public required string UserId { get; set; }
        public required string Username { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
