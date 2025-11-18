using System.ComponentModel.DataAnnotations;

namespace GoalGrow.Entity.ValueObjects
{
    /// <summary>
    /// Value Object per rappresentare informazioni di contatto
    /// </summary>
    public record ContactInfo
    {
        [MaxLength(100)]
        public string Email { get; init; } = string.Empty;

        [MaxLength(50)]
        public string PhoneNumber { get; init; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; init; } = string.Empty;

        public ContactInfo() { }

        public ContactInfo(string email, string phoneNumber, string address = "")
        {
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
        }

        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
        public bool HasPhone => !string.IsNullOrWhiteSpace(PhoneNumber);
        public bool HasAddress => !string.IsNullOrWhiteSpace(Address);

        public override string ToString()
        {
            var parts = new List<string>();
            if (HasEmail) parts.Add(Email);
            if (HasPhone) parts.Add(PhoneNumber);
            if (HasAddress) parts.Add(Address);
            return string.Join(" | ", parts);
        }
    }
}
