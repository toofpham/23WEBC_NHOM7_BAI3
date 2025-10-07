using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using TachLayout.Models;
using TachLayout.Services;
using Microsoft.Data.SqlClient;

namespace TachLayout.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbService _db;

        public HomeController(ILogger<HomeController> logger, DbService db)
        {
            _logger = logger;
            _db = db;
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

        [HttpGet("faqs")]
        public IActionResult Faqs()
        {
            ViewData["Title"] = "Faqs";
            ViewData["PageName"] = "Faqs";

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}