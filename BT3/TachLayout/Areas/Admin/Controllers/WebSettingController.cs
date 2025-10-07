using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TachLayout.Filters;
using TachLayout.Models;

namespace TachLayout.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [AdminAuthorize]
    public class WebSettingController : Controller
    {
        private readonly string connectionString;

        public WebSettingController(IConfiguration configuration)
        {
            // ✅ Lấy từ appsettings.json
            connectionString = configuration.GetConnectionString("QuanLyConn");
        }

        private WebSetting GetWebSetting()
        {
            WebSetting setting = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT TOP 1 * FROM WebSetting";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    setting = new WebSetting
                    {
                        WebSettingID = (int)reader["WebSettingID"],
                        Logo = reader["Logo"].ToString(),
                        TenSite = reader["TenSite"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        HotLine = reader["HotLine"].ToString()
                    };
                }
            }
            return setting;
        }

        [HttpGet("WebSetting")]
        public IActionResult WebSetting()
        {
            ViewData["Title"] = "Admin - WebSetting";
            var setting = GetWebSetting();

            return View(setting);
        }

        // ✅ Hiển thị form chỉnh sửa
        [HttpGet("EditWebSetting")]
        public IActionResult EditWebSetting()
        {
            var setting = GetWebSetting();
            return View(setting);
        }

        // ✅ Cập nhật dữ liệu
        [HttpPost("EditWebSetting")]
        public IActionResult EditWebSetting(WebSetting model)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE WebSetting 
                               SET Logo = @Logo, TenSite = @TenSite, DiaChi = @DiaChi, Email = @Email, HotLine = @HotLine";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Logo", model.Logo ?? "");
                cmd.Parameters.AddWithValue("@TenSite", model.TenSite ?? "");
                cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? "");
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@HotLine", model.HotLine ?? "");

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            // Quay về trang hiển thị
            return RedirectToAction("WebSetting");
        }
    }
}
