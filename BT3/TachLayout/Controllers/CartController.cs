using Microsoft.AspNetCore.Mvc;

namespace TachLayout.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            
            return View("Index");
        }
    }
}
