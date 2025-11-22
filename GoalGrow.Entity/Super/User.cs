using GoalGrow.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoalGrow.Entity.Super
{
    public abstract class User
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public UserType UserType { get; set; } = default!;
        
        /// <summary>
        /// Keycloak Subject ID (sub claim) - Used for synchronization with Keycloak
        /// </summary>
        public string? KeycloakSubjectId { get; set; }

        protected User(string firstName, string lastName, string phoneNumber, string emailAddress, UserType userType)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            EmailAddress = emailAddress;
            UserType = userType;
        }
    }
}
