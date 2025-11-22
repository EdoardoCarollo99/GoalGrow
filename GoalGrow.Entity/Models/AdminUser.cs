using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta un utente amministratore della piattaforma GoalGrow
    /// </summary>
    public class AdminUser : User
    {
        /// <summary>
        /// Ruolo dell'amministratore (es: SuperAdmin, Support, Finance)
        /// </summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Permessi assegnati (JSON array o stringa separata da virgole)
        /// </summary>
        public string Permissions { get; set; } = string.Empty;

        /// <summary>
        /// Dipartimento di appartenenza
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Data di assunzione
        /// </summary>
        public DateTime? HireDate { get; set; }

        /// <summary>
        /// Note interne sull'amministratore
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Se true, l'admin ha accesso completo al sistema
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        public AdminUser(
            string firstName,
            string lastName,
            string phoneNumber,
            string emailAddress,
            string role,
            UserType userType = UserType.AdminUser)
            : base(firstName, lastName, phoneNumber, emailAddress, userType)
        {
            Role = role;
            IsSuperAdmin = role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
