using Microsoft.AspNetCore.Mvc;
using System.Data;
using TachLayout.Services;
using Microsoft.Data.SqlClient;

namespace TachLayout.Controllers
{
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly DbService _db;

        public ProductController(DbService db)
        {
            _db = db;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewData["Title"] = "Sản phẩm";
            ViewData["PageName"] = "Products";

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

        [HttpGet("{id}")]
        public IActionResult Details(string id)
        {
            ViewData["Title"] = "Chi tiết sản phẩm";
            ViewData["PageName"] = "ProductDetails";

            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                string sql = "SELECT * FROM SanPham WHERE MaSP = @MaSP";
                var parameters = new[] { new SqlParameter("@MaSP", id) };
                DataTable dt = _db.GetDataTable(sql, parameters);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return NotFound();
                }

                return View(dt.Rows[0]);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Lỗi SQL: {ex.Message}");
                return View("Error", new { Message = "Có lỗi khi tải chi tiết sản phẩm." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi chung: {ex.Message}");
                return View("Error", new { Message = "Có lỗi không xác định." });
            }
        }
    }
}