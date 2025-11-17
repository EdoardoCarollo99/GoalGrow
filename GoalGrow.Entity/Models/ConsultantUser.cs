using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.Models
{
    public class ConsultantUser : User
    {
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Specialization { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Biography { get; set; } = string.Empty;

        public DateTime? CertificationDate { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(0, 100)]
        public decimal CommissionRate { get; set; } = 0m; // Percentuale commissione sugli investimenti

        public int YearsOfExperience { get; set; } = 0;

        // Rating e recensioni
        public decimal AverageRating { get; set; } = 0m;

        public int TotalReviews { get; set; } = 0;

        // Relazioni
        public virtual ICollection<UserConsultantRelationship> Clients { get; set; } = new List<UserConsultantRelationship>();
        public virtual ICollection<CommissionTransaction> Commissions { get; set; } = new List<CommissionTransaction>();

        public ConsultantUser(string firstName, string lastName, string phoneNumber, 
            string emailAddress, string licenseNumber, UserType userType = UserType.ConsultantUser)
            : base(firstName, lastName, phoneNumber, emailAddress, userType)
        {
            LicenseNumber = licenseNumber;
        }
    }
}
