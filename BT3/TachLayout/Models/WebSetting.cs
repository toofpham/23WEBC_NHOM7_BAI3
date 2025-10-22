using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TachLayout.Models
{
    [Table("WebSetting")]
    public class WebSetting
    {
        [Key] // Khóa chính
        public int WebSettingID { get; set; }
        public string TenSite { get; set; }
        public string Logo { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string HotLine { get; set; }
        public int DungLuongToiDa { get; set; }
        public int RequestToiDa { get; set; }
    }
}
