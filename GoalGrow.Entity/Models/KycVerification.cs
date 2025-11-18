using GoalGrow.Entity.Common;
using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoalGrow.Entity.Models
{
    /// <summary>
    /// Rappresenta il processo di verifica KYC (Know Your Customer) di un utente
    /// </summary>
    public class KycVerification : AuditableEntity
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public KycStatus Status { get; set; } = KycStatus.Pending;

        // Document Information
        [MaxLength(50)]
        public string DocumentType { get; set; } = string.Empty; // ID, Passport, Driver License

        [MaxLength(100)]
        public string DocumentNumber { get; set; } = string.Empty;

        public DateTime? DocumentExpiryDate { get; set; }

        [MaxLength(500)]
        public string DocumentFrontImageUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DocumentBackImageUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string SelfieImageUrl { get; set; } = string.Empty;

        // Address Verification
        [MaxLength(500)]
        public string ProofOfAddressImageUrl { get; set; } = string.Empty; // Utility bill, bank statement

        public DateTime? AddressVerificationDate { get; set; }

        // Verification Dates
        public DateTime? SubmittedAt { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public DateTime? RejectedAt { get; set; }

        [MaxLength(1000)]
        public string RejectionReason { get; set; } = string.Empty;

        // Verified By (Admin or Automated System)
        public Guid? VerifiedByUserId { get; set; }

        [MaxLength(100)]
        public string VerificationMethod { get; set; } = string.Empty; // Manual, Automated, Hybrid

        // External KYC Provider (if using third-party service like Onfido, Jumio)
        [MaxLength(200)]
        public string ExternalProviderId { get; set; } = string.Empty;

        [MaxLength(100)]
        public string ExternalReferenceId { get; set; } = string.Empty;

        // Risk Assessment
        public int RiskScore { get; set; } = 0; // 0-100

        [MaxLength(50)]
        public string RiskLevel { get; set; } = "Unknown"; // Low, Medium, High

        // PEP (Politically Exposed Person) Check
        public bool IsPoliticallyExposed { get; set; } = false;

        [MaxLength(1000)]
        public string PepDetails { get; set; } = string.Empty;

        // Sanctions Check
        public bool IsOnSanctionsList { get; set; } = false;

        [MaxLength(1000)]
        public string SanctionsDetails { get; set; } = string.Empty;

        // Navigation
        [ForeignKey(nameof(UserId))]
        public virtual InversotorUser User { get; set; } = default!;

        [ForeignKey(nameof(VerifiedByUserId))]
        public virtual User? VerifiedBy { get; set; }

        // Computed Properties
        [NotMapped]
        public bool IsVerified => Status == KycStatus.Verified;

        [NotMapped]
        public bool IsRejected => Status == KycStatus.Rejected;

        [NotMapped]
        public bool IsPending => Status == KycStatus.Pending || Status == KycStatus.UnderReview;

        [NotMapped]
        public int DaysSinceSubmission => SubmittedAt.HasValue 
            ? (DateTime.UtcNow - SubmittedAt.Value).Days 
            : 0;
    }
}
