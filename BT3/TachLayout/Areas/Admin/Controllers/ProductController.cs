#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TachLayout.Filters;

using TachLayout.Models;

namespace TachLayout.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ProductController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("QuanLyConn")!;
            List<Product> list = new List<Product>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM SanPham";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string hinhValue = reader["Hinh"] != DBNull.Value? reader["Hinh"].ToString() ?? "": "";
                    // Nếu không có hình thì gán ảnh mặc định
                    string imagePath = string.IsNullOrWhiteSpace(hinhValue)
                        ? "product/default.jpg"
                        : "product/" + hinhValue.Trim();
                    list.Add(new Product
                    {
                        MaSP = (int)reader["MaSP"],
                        TenSP = reader["TenSP"].ToString() ?? "",
                        Gia = (decimal)reader["Gia"],
                        GiaKM = (decimal)reader["GiaKM"],
                        MaDanhMuc = (int)reader["MaDanhMuc"],
                        Hinh = reader["Hinh"].ToString() ?? "",
                        MoTa = reader["MoTa"].ToString() ?? "",
                        TagName = reader["TagName"].ToString() ?? ""
                    });
                }
                conn.Close();
            }

            return View(list);
        }

        // ✅ 1. Hiển thị form thêm sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Areas/Admin/Views/Product/Create.cshtml");
        }

        // ✅ 2. Xử lý khi bấm nút "Thêm"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model, IFormFile? HinhFile)
        {
            string connectionString = _configuration.GetConnectionString("QuanLyConn")!;
            string fileName = "";

            // ✅ Lưu ảnh nếu có chọn
            if (HinhFile != null)
            {
                fileName = Path.GetFileName(HinhFile.FileName);
                string uploadPath = Path.Combine(_env.WebRootPath, "images/product", fileName);
                using (var stream = new FileStream(uploadPath, FileMode.Create))
                {
                    HinhFile.CopyTo(stream);
                }
            }

            // ✅ Lưu sản phẩm vào bảng SanPham
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO SanPham (TenSP, Gia, GiaKM, MaDanhMuc, Hinh, MoTa, TagName)
               VALUES (@TenSP, @Gia, @GiaKM, @MaDanhMuc, @Hinh, @MoTa, @TagName)";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@TenSP", model.TenSP);
                cmd.Parameters.AddWithValue("@Gia", model.Gia);
                cmd.Parameters.AddWithValue("@GiaKM", model.GiaKM);
                cmd.Parameters.AddWithValue("@MaDanhMuc", model.MaDanhMuc);
                cmd.Parameters.AddWithValue("@Hinh", fileName ?? "");
                cmd.Parameters.AddWithValue("@MoTa", model.MoTa ?? "");
                cmd.Parameters.AddWithValue("@TagName", model.TagName ?? "");

                conn.Open();//
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            TempData["Success"] = "✅ Thêm sản phẩm thành công!";
            return RedirectToAction("Index"); // Quay về trang danh sách
        }
    }
}
