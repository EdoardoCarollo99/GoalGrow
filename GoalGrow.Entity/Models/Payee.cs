using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoalGrow.Entity.Super;

namespace GoalGrow.Entity.Models
{
    public class Payee
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Category { get; set; } = string.Empty;

        [MaxLength(100)]
        public string AccountNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TaxCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = default!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public Payee(string name, Guid userId)
        {
            Name = name;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
