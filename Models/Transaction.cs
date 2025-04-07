using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty; // Income or Expense

        [Required]
        public string Category { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int WalletId { get; set; }

        // Navigation properties
        public required User User { get; set; }
        public required Wallet Wallet { get; set; }
    }
} 