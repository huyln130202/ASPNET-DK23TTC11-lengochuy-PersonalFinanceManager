using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly, Yearly

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation property
        public required User User { get; set; }
    }
} 