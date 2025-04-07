using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
} 