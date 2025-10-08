namespace TachLayout.Models
{
    public class CartItemModel
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; }
        public decimal Gia { get; set; }
        public decimal GiaKM { get; set; }
        public int SoLuong { get; set; }
        public string Hinh { get; set; } = string.Empty;

        public decimal ThanhTien
        {
            get { return (GiaKM > 0 ? GiaKM : Gia) * SoLuong; }
            set { }
        }
    }
    
}
