using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh Mục")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Display(Name = "Số Tiền")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn chu kỳ")]
        [Display(Name = "Chu Kỳ")]
        public string Period { get; set; } = string.Empty; // Daily, Weekly, Monthly, Yearly

        [Display(Name = "Ngày Bắt Đầu")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Ngày Kết Thúc")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Mô Tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? UpdatedAt { get; set; }

        // Foreign key
        public int UserId { get; set; }

        // Navigation property
        public User? User { get; set; }
    }
} 