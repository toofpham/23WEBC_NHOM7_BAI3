using Microsoft.AspNetCore.Mvc;
using System.Data;
using TachLayout.Extensions;
using TachLayout.Models;
using TachLayout.Services;

namespace TachLayout.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DbService _db;
        public ShoppingCartController(DbService db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            return View("~/Views/Cart/Index.cshtml", cart);
        }
        public IActionResult AddToCart(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Lấy sản phẩm từ DB
            string sql = $"SELECT * FROM SanPham WHERE MaSP = {id}";
            DataTable dt = _db.GetDataTable(sql);
            if (dt.Rows.Count == 0)
                return NotFound();

            var row = dt.Rows[0];
            var existing = cart.FirstOrDefault(x => x.MaSP == id);

            if (existing != null)
            {
                existing.SoLuong++;
            }
            else
            {
                var item = new CartItemModel
                {
                    MaSP = (int)row["MaSP"],
                    TenSP = row["TenSP"].ToString(),
                    Gia = (decimal)row["Gia"],
                    GiaKM = row["GiaKM"] == DBNull.Value ? 0 : (decimal)row["GiaKM"],
                    SoLuong = 1,
                    Hinh = row["Hinh"].ToString()
                };
                cart.Add(item);
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // Sau khi thêm, chuyển hướng lại trang giỏ hàng
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            cart.RemoveAll(x => x.MaSP == id);
            HttpContext.Session.SetObjectAsJson("Cart", cart);
            return RedirectToAction("Index");
        }

    }
}
