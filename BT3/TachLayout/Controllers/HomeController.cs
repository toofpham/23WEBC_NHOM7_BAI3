using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;
using TachLayout.Models;
using TachLayout.Services;

namespace TachLayout.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbService _db;
        private readonly IConfiguration _configuration; 

        public HomeController(ILogger<HomeController> logger, DbService db, IConfiguration configuration)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration; 
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Trang chủ";
            ViewData["PageName"] = "Home";

            try
            {
                string sql = "SELECT * FROM SanPham";
                DataTable dt = _db.GetDataTable(sql);
                return View(dt);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL: {ex.Message}");
                return View("Error", new { Message = "Có lỗi khi tải sản phẩm. Vui lòng thử lại sau." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi chung: {ex.Message}");
                return View("Error", new { Message = "Có lỗi không xác định. Vui lòng liên hệ hỗ trợ." });
            }
        }

        [HttpGet("search")]
        public IActionResult Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                TempData["SearchMessage"] = "Vui lòng nhập tên sản phẩm để tìm kiếm.";
                return RedirectToAction("Index");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT * FROM SANPHAM WHERE TenSP LIKE @keyword";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            ViewBag.Keyword = keyword;
            return View("Index", dt);
        }
        [HttpGet("about")]
        public IActionResult About()
        {
            ViewData["Title"] = "About";
            ViewData["PageName"] = "About";

            return View();
        }

        [HttpGet("contact")]
        public IActionResult Contact()
        {
            ViewData["Title"] = "Contact";
            ViewData["PageName"] = "Contact";

            return View();
        }

    }
}
