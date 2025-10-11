using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text.Json;
using TachLayout.Models;

public class CartController : Controller
{
    private readonly IConfiguration _configuration;

    public CartController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        // Hiển thị thông tin user nếu đã đăng nhập
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId != null)
        {
            ViewData["IsLoggedIn"] = true;
            ViewData["Username"] = HttpContext.Session.GetString("Username");
        }
        else
        {
            ViewData["IsLoggedIn"] = false;
        }

        var cart = GetCart();
        return View(cart);
    }

    // Action Thanh toán trực tiếp
    [HttpPost]
    public IActionResult Checkout()
    {
        // Kiểm tra đăng nhập
        var userId = HttpContext.Session.GetInt32("UserID");
        if (userId == null)
        {
            TempData["ErrorMessage"] = "Vui lòng đăng nhập để thanh toán";
            return RedirectToAction("Login", "Account");
        }

        var cart = GetCart();
        if (cart == null || !cart.Any())
        {
            TempData["ErrorMessage"] = "Giỏ hàng trống";
            return RedirectToAction("Index");
        }

        // Lấy thông tin user từ database
        var userInfo = GetUserInfo(userId.Value);
        if (userInfo == null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng";
            return RedirectToAction("Index");
        }

        // Tạo hóa đơn
        try
        {
            int maHD = CreateOrder(userId.Value, userInfo, cart);

            if (maHD > 0)
            {
                // Xóa giỏ hàng
                ClearCart();

                TempData["OrderSuccess"] = $"Đặt hàng thành công! Mã đơn hàng: #{maHD}. Đơn hàng của bạn đang chờ xác nhận.";
                return RedirectToAction("OrderSuccess");
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi đặt hàng";
                return RedirectToAction("Index");
            }
        }
        catch (SqlException ex)
        {
            TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    private UserInfo GetUserInfo(int userId)
    {
        using (var conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            conn.Open();
            string query = "SELECT UserID, UserName FROM [User] WHERE UserID = @UserID";

            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserInfo
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["UserName"].ToString(),
                            Email = "",
                            Phone = "",
                            Address = ""
                        };
                    }
                }
            }
        }
        return null;
    }

    private int CreateOrder(int userId, UserInfo userInfo, List<CartItemModel> cart)
    {
        if (cart == null || !cart.Any())
            throw new ArgumentException("Giỏ hàng trống");

        decimal tongTien = cart.Sum(x => x.ThanhTien);

        using (var conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
        {
            conn.Open();

            using (var tran = conn.BeginTransaction())
            {
                try
                {
                    // 1) Insert HoaDon và lấy MaHD bằng OUTPUT INSERTED.MaHD
                    string insertHoaDon = @"
                    INSERT INTO HoaDon (UserID, TenNguoiNhan, SDT, DiaChi, NgayLap, TongTien, TrangThai)
                    OUTPUT INSERTED.MaHD
                    VALUES (@UserID, @TenNguoiNhan, @SDT, @DiaChi, GETDATE(), @TongTien, N'Chờ xác nhận');";

                    int maHD;
                    using (var cmd = new SqlCommand(insertHoaDon, conn, tran))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@TenNguoiNhan", (object)userInfo.Username ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@SDT", (object)userInfo.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiaChi", (object)userInfo.Address ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TongTien", tongTien);

                        var result = cmd.ExecuteScalar();
                        if (result == null)
                            throw new Exception("Không tạo được hóa đơn (maHD null).");

                        maHD = Convert.ToInt32(result);
                    }

                    // 2) Insert ChiTietHoaDon
                    string insertCT = @"
                    INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia)
                    VALUES (@MaHD, @MaSP, @SoLuong, @DonGia);";

                    foreach (var item in cart)
                    {
                        using (var cmdCt = new SqlCommand(insertCT, conn, tran))
                        {
                            cmdCt.Parameters.AddWithValue("@MaHD", maHD);
                            cmdCt.Parameters.AddWithValue("@MaSP", item.MaSP);
                            cmdCt.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdCt.Parameters.AddWithValue("@DonGia", item.Gia);
                            cmdCt.ExecuteNonQuery();
                        }
                    }

                    tran.Commit();
                    return maHD;
                }
                catch
                {
                    try { tran.Rollback(); } catch { }
                    throw;
                }
            }
        }
    }


    //private int GetNextMaHD()
    //{
    //    using (var conn = new SqlConnection(_configuration.GetConnectionString("QuanLyConn")))
    //    {
    //        conn.Open();
    //        string query = "SELECT ISNULL(MAX(MaHD), 0) + 1 FROM HoaDon";
    //        using (var cmd = new SqlCommand(query, conn))
    //        {
    //            return Convert.ToInt32(cmd.ExecuteScalar());
    //        }
    //    }
    //}

    private List<CartItemModel> GetCart()
    {
        var cartJson = HttpContext.Session.GetString("Cart");
        return string.IsNullOrEmpty(cartJson)
            ? new List<CartItemModel>()
            : JsonSerializer.Deserialize<List<CartItemModel>>(cartJson);
    }

    private void ClearCart()
    {
        HttpContext.Session.Remove("Cart");
    }

    // Các action khác (UpdateQuantity, Remove, v.v.)
    [HttpPost]
    public IActionResult UpdateQuantity(int id, int change, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.MaSP == id);

        if (item != null)
        {
            if (change == 1) // Tăng
            {
                item.SoLuong = quantity + 1;
            }
            else if (change == -1 && quantity > 1) // Giảm
            {
                item.SoLuong = quantity - 1;
            }

            item.ThanhTien = item.SoLuong * item.Gia;

            // Lưu lại vào session
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Remove(int id)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.MaSP == id);

        if (item != null)
        {
            cart.Remove(item);

            // Lưu lại vào session
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", cartJson);
        }

        return RedirectToAction("Index");
    }

    public IActionResult OrderSuccess()
    {
        return View();
    }
}