using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceManager.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ví")]
        [Display(Name = "Tên Ví")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Số Dư")]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại ví")]
        [Display(Name = "Loại Ví")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Mô Tả")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Ngày Tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Ngày Cập Nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Trạng Thái")]
        public bool IsActive { get; set; } = true;

        // Foreign keys
        public int UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
} 