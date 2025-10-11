using System.ComponentModel.DataAnnotations;

namespace TachLayout.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100)]
        public string Pass { get; set; }

        public int TrangThai { get; set; } = 1; // 1 = hoạt động, 0 = khóa

        [StringLength(20)]
        public string Role { get; set; } = "Customer"; // mặc định sẽ là customer tức có nghĩa khi đăng kí bạn sẽ luôn được giữ vai trò khách hàng
    }
}
