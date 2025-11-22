using System.ComponentModel.DataAnnotations;

namespace GoalGrow.API.DTOs.Requests
{
    /// <summary>
    /// Richiesta di login con credenziali utente
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
