using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Display(Name = "Số Tiền")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giao dịch")]
        [Display(Name = "Loại Giao Dịch")]
        public string Type { get; set; } = string.Empty; // Income or Expense

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh Mục")]
        public string Category { get; set; } = string.Empty;

        [Display(Name = "Mô Tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ngày Giao Dịch")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Foreign keys
        public int UserId { get; set; }
        public int WalletId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Wallet? Wallet { get; set; }
    }
} 