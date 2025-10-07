namespace TachLayout.Models
{
    public class Product
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = string.Empty;
        public decimal Gia { get; set; }
        public decimal GiaKM { get; set; }
        public int MaDanhMuc { get; set; }
        public string Hinh { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
    }
}