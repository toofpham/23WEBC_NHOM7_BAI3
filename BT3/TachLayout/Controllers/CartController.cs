using Microsoft.AspNetCore.Mvc;
using System.Data;
using TachLayout.Extensions;
using TachLayout.Models;

namespace TachLayout.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Trả về view kèm model
            return View("Index", cart);
        }
        [HttpPost("D/A")]
        public IActionResult UpdateQuantity(int id, string change)
        {
            // đọc giỏ hàng hiện tại từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            var item = cart.FirstOrDefault(x => x.MaSP == id);

            if (item != null)
            {
                if (change == "+1") item.SoLuong++;
                else if (change == "-1" && item.SoLuong > 1) item.SoLuong--;
                // không gán ThanhTien nữa, vì là thuộc tính tính toán
            }

            // lưu lại vào session
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            // quay lại trang giỏ hàng
            return RedirectToAction("Index", "Cart");
        }
        [HttpGet("remove/{id}")]
        public IActionResult Remove(int id)
        {
            // Lấy giỏ hàng hiện tại từ session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm sản phẩm có mã tương ứng
            var itemToRemove = cart.FirstOrDefault(x => x.MaSP == id);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                // Lưu lại session sau khi xóa
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }

            // Quay lại trang giỏ hàng
            return RedirectToAction("Index");
        }


    }
}
