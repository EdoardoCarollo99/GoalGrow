using GoalGrow.Entity.Enums;
using GoalGrow.Entity.Super;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GoalGrow.Entity.Models
{
    public class InversotorUser : User
    {
        public string FiscalCode { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.MinValue;

        // Wallet virtuale - saldo disponibile nel conto aziendale per questo utente
        [Column(TypeName = "decimal(18,2)")]
        public decimal VirtualWalletBalance { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDeposited { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalWithdrawn { get; set; } = 0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInvested { get; set; } = 0m;

        // Compliance
        public virtual KycVerification? KycVerification { get; set; }

        // Relazioni finanziarie
        public virtual UserConsultantRelationship? ConsultantRelationship { get; set; }
        public virtual ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();
        public virtual ICollection<Investment> Investments { get; set; } = new List<Investment>();
        public virtual ICollection<FundMovement> FundMovements { get; set; } = new List<FundMovement>();
        public virtual ICollection<CommissionTransaction> CommissionTransactions { get; set; } = new List<CommissionTransaction>();

        // Profilo di rischio
        public virtual RiskProfile? RiskProfile { get; set; }

        // Gamification
        public virtual UserLevel? UserLevel { get; set; }
        public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
        public virtual ICollection<UserChallenge> UserChallenges { get; set; } = new List<UserChallenge>();

        // Notifiche
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public InversotorUser(string firstName, string lastName, string phoneNumber, string emailAddress, string fiscalCode, DateTime birthDate, UserType userType = UserType.InvestorUser)
            : base(firstName, lastName, phoneNumber, emailAddress, userType)
        {
            FiscalCode = fiscalCode;
            BirthDate = birthDate;
        }
    }
}
