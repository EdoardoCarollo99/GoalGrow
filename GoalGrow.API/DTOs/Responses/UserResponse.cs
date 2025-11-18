namespace GoalGrow.API.DTOs.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string KeycloakSubjectId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class InvestorUserResponse : UserResponse
    {
        public string FiscalCode { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public decimal VirtualWalletBalance { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal TotalInvested { get; set; }
        public bool HasKycVerification { get; set; }
        public string? KycStatus { get; set; }
    }

    public class ConsultantUserResponse : UserResponse
    {
        public string LicenseNumber { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public decimal CommissionRate { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
