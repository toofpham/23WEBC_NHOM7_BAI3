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

    // ----- DASHBOARD (chỉ menu + thông báo) -----
    [HttpGet("")]
    [HttpGet("Index")]
    [AdminAuthorize]
    public IActionResult Index()
    {
        ViewData["Title"] = "Admin - Quản trị";
        // Dashboard chỉ cần hiển thị menu / thông báo -> không cần chuyền dữ liệu đơn hàng
        return View("~/Areas/Admin/Views/Admin/Index.cshtml"); // file view mới Dashboard.cshtml
    }

    // ----- DANH SÁCH ĐƠN HÀNG (tách riêng) -----
    [HttpGet("DonHang")]
    [AdminAuthorize]
    public IActionResult DonHang()
    {
        ViewData["Title"] = "Quản lý Đơn Hàng";

        // Lấy danh sách đơn hàng chờ xác nhận
        DataTable donHangChoXacNhan = GetDonHangByStatus("Chờ xác nhận");

        // Lấy danh sách đơn hàng đã xác nhận
        DataTable donHangDaXacNhan = GetDonHangByStatus("Đã xác nhận");

        ViewData["DonHangChoXacNhan"] = donHangChoXacNhan;
        ViewData["DonHangDaXacNhan"] = donHangDaXacNhan;

        return View("~/Areas/Admin/Views/Admin/DonHang.cshtml"); // file view mới DonHang.cshtml
    }

    // ----- POST: xác nhận đơn hàng -----
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

        // Chuyển về trang danh sách đơn hàng
        return RedirectToAction("DonHang");
    }

    // ----- POST: huỷ đơn hàng -----
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

        // Chuyển về trang danh sách đơn hàng
        return RedirectToAction("DonHang");
    }

    // ----- Helpers (giữ nguyên) -----
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

    // GET: /Admin/ChiTietDonHang/{maHD}
    [HttpGet("ChiTietDonHang/{maHD}")]
    [AdminAuthorize]
    public IActionResult ChiTietDonHang(int maHD)
    {
        ViewData["Title"] = $"Chi tiết đơn hàng #{maHD}";

        // Lấy thông tin hóa đơn (header)
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

        // Lấy chi tiết hóa đơn (sản phẩm)
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

        ViewData["HoaDon"] = dtHeader.Rows[0];   // 1 dòng header
        ViewData["ChiTietHoaDon"] = dtItems;     // nhiều dòng chi tiết

        return View("~/Areas/Admin/Views/Admin/ChiTietDonHang.cshtml");
    }



}
