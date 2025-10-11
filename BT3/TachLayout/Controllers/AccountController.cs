using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TachLayout.Services;
using TachLayout.Models;
using System.Reflection;

namespace TachLayout.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly DbService _db;

        public AccountController(DbService db)
        {
            _db = db;
        }

        // ========== GET: /account/register ==========
        [HttpGet("register")]
        public IActionResult Register()
        {
            ViewData["Title"] = "Đăng ký tài khoản";
            return View();
        }

        // ========== POST: /account/register ==========
        [HttpPost("register")]
        public IActionResult Register(string username, string pass, string confirmPass)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(confirmPass))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (pass != confirmPass)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            try
            {
                // TẠM THỜI: Lấy UserID tiếp theo
                int nextUserID = GetNextUserID();

                // Kiểm tra trùng username
                string checkSql = "SELECT COUNT(*) FROM [User] WHERE UserName = @UserName";
                var checkParams = new[] { new SqlParameter("@UserName", username) };
                DataTable checkResult = _db.GetDataTable(checkSql, checkParams);

                int count = Convert.ToInt32(checkResult.Rows[0][0]);
                if (count > 0)
                {
                    ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                    return View();
                }

                // TẠM THỜI: Thêm user với UserID thủ công
                string insertSql = @"INSERT INTO [User] (UserID, UserName, Pass, TrangThai, Role)
                             VALUES (@UserID, @UserName, @Pass, 1, 'Customer')";
                var insertParams = new[]
                {
            new SqlParameter("@UserID", nextUserID),
            new SqlParameter("@UserName", username),
            new SqlParameter("@Pass", pass)
        };
                int rows = _db.ExecuteNonQuery(insertSql, insertParams);

                if (rows > 0)
                {
                    TempData["Success"] = "Đăng ký thành công! Mời bạn đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Không thể tạo tài khoản. Vui lòng thử lại.";
                    return View();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL: {ex.Message}");
                ViewBag.Error = "Có lỗi khi đăng ký. Vui lòng thử lại.";
                return View();
            }
        }

       
        private int GetNextUserID()
        {
            try
            {
                string sql = "SELECT ISNULL(MAX(UserID), 0) + 1 FROM [User]";
                DataTable dt = _db.GetDataTable(sql);

                if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    return Convert.ToInt32(dt.Rows[0][0]);
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy UserID tiếp theo: {ex.Message}");
                return 1;
            }
        }

        // ========== GET: /account/login ==========
        [HttpGet("login")]
        public IActionResult Login()
        {
            ViewData["Title"] = "Đăng nhập";
            return View();
        }

        // ========== POST: /account/login ==========
        [HttpPost("login")]
        public IActionResult Login(string username, string pass)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(pass))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return View();
            }

            try
            {
                // Sửa: Dùng UserName thay vì Username (theo cấu trúc bảng)
                string sql = "SELECT * FROM [User] WHERE UserName = @UserName AND Pass = @Pass";
                var parameters = new[]
                {
                    new SqlParameter("@UserName", username), // Sửa thành @UserName
                    new SqlParameter("@Pass", pass)
                };

                DataTable dt = _db.GetDataTable(sql, parameters);

                if (dt.Rows.Count == 0)
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu.";
                    return View();
                }

                var row = dt.Rows[0];
                string role = row["Role"].ToString();
                int trangthai = Convert.ToInt32(row["TrangThai"]);
                int userID = Convert.ToInt32(row["UserID"]);

                if (trangthai == 0)
                {
                    ViewBag.Error = "Tài khoản đã bị khóa.";
                    return View();
                }

                // Lưu session
                HttpContext.Session.SetInt32("UserID", userID);
                HttpContext.Session.SetString("Username", username);
                HttpContext.Session.SetString("Role", role);

                if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    HttpContext.Session.SetString("AdminSession", username);
                    HttpContext.Session.SetString("AdminName", username);
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL: {ex.Message}");
                ViewBag.Error = "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.";
                return View();
            }
        }

        // ========== GET: /account/logout ==========
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}