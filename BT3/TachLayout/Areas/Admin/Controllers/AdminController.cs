using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using TachLayout.Filters;
using TachLayout.Models;
using TachLayout.Services;

[Area("Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly DbService _db;
    private readonly IConfiguration _configuration;

    public AdminController(DbService db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
    }

    
    [HttpGet("")]
    [HttpGet("Index")]
    [AdminAuthorize]
    public IActionResult Index()
    {
        ViewData["Title"] = "Admin - Quản trị";
       
        return View("~/Areas/Admin/Views/Admin/Index.cshtml"); 
    }

    [HttpGet("DonHang")]
    [AdminAuthorize]
    public IActionResult DonHang()
    {
        ViewData["Title"] = "Quản lý Đơn Hàng";

                DataTable donHangChoXacNhan = GetDonHangByStatus("Chờ xác nhận");

                DataTable donHangDaXacNhan = GetDonHangByStatus("Đã xác nhận");

        ViewData["DonHangChoXacNhan"] = donHangChoXacNhan;
        ViewData["DonHangDaXacNhan"] = donHangDaXacNhan;

        return View("~/Areas/Admin/Views/Admin/DonHang.cshtml");     }

        [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("XacNhanDonHang")]
    public IActionResult XacNhanDonHang(int maHD)
    {
        try
        {
            UpdateOrderStatus(maHD, "Đã xác nhận");
            TempData["SuccessMessage"] = $"Đã xác nhận đơn hàng #{maHD}";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Lỗi khi xác nhận đơn hàng: {ex.Message}";
        }

                return RedirectToAction("DonHang");
    }

        [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("HuyDonHang")]
    public IActionResult HuyDonHang(int maHD)
    {
        try
        {
            UpdateOrderStatus(maHD, "Đã hủy");
            TempData["SuccessMessage"] = $"Đã hủy đơn hàng #{maHD}";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Lỗi khi hủy đơn hàng: {ex.Message}";
        }

                return RedirectToAction("DonHang");
    }

        private DataTable GetDonHangByStatus(string trangThai)
    {
        DataTable dt = new DataTable();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            string query = @"
                SELECT 
                    hd.MaHD, 
                    hd.TenNguoiNhan, 
                    hd.SDT, 
                    hd.DiaChi, 
                    hd.NgayLap, 
                    hd.TongTien, 
                    hd.TrangThai,
                    u.UserName
                FROM HoaDon hd 
                LEFT JOIN [User] u ON hd.UserID = u.UserID
                WHERE hd.TrangThai = @TrangThai 
                ORDER BY hd.NgayLap DESC";

            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            da.SelectCommand.Parameters.AddWithValue("@TrangThai", trangThai);
            da.Fill(dt);
        }
        return dt;
    }

    private void UpdateOrderStatus(int maHD, string trangThai)
    {
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            conn.Open();
            string query = "UPDATE HoaDon SET TrangThai = @TrangThai WHERE MaHD = @MaHD";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@MaHD", maHD);
                cmd.ExecuteNonQuery();
            }
        }
    }

        [HttpGet("ChiTietDonHang/{maHD}")]
    [AdminAuthorize]
    public IActionResult ChiTietDonHang(int maHD)
    {
        ViewData["Title"] = $"Chi tiết đơn hàng #{maHD}";

                DataTable dtHeader = new DataTable();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            string qHeader = @"
            SELECT hd.MaHD, hd.TenNguoiNhan, hd.SDT, hd.DiaChi, hd.NgayLap, hd.TongTien, hd.TrangThai,
                   u.UserName
            FROM HoaDon hd
            LEFT JOIN [User] u ON hd.UserID = u.UserID
            WHERE hd.MaHD = @MaHD";
            SqlDataAdapter da = new SqlDataAdapter(qHeader, conn);
            da.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);
            da.Fill(dtHeader);
        }

                DataTable dtItems = new DataTable();
        using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            string qItems = @"
            SELECT ct.ChiTietID, ct.MaSP, sp.TenSP,
                   ct.SoLuong, ct.DonGia, (ct.SoLuong * ct.DonGia) AS ThanhTien
            FROM ChiTietHoaDon ct
            LEFT JOIN SanPham sp ON ct.MaSP = sp.MaSP
            WHERE ct.MaHD = @MaHD";
            SqlDataAdapter da = new SqlDataAdapter(qItems, conn);
            da.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);
            da.Fill(dtItems);
        }

        if (dtHeader.Rows.Count == 0)
        {
            TempData["ErrorMessage"] = $"Không tìm thấy hóa đơn #{maHD}.";
            return RedirectToAction("DonHang");
        }

        ViewData["HoaDon"] = dtHeader.Rows[0];           
        ViewData["ChiTietHoaDon"] = dtItems;     
        return View("~/Areas/Admin/Views/Admin/ChiTietDonHang.cshtml");
    }



}
