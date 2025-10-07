using Microsoft.AspNetCore.Mvc;

namespace TachLayout.Areas.API.Controllers
{
    [Area("API")]
    [Route("api")]
    [ApiController]
    public class ApiController : Controller
    {
        [HttpGet("hello")]
        public IActionResult GetHello()
        {
            return Ok("Xin chào từ API trong Area API!");
        }
    }
}
