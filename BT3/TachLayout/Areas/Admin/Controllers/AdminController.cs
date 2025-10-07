using Microsoft.AspNetCore.Mvc;
using TachLayout.Filters;
using TachLayout.Models;

[Area("Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly string adminUser = "admin";
    private readonly string adminPass = "123";
    [HttpGet("")]
    [AdminAuthorize]
    public IActionResult Index()
    {
        ViewData["Title"] = "Admin - Quản trị";
        return View();
    }

    [HttpGet("Login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost("Login")]
    public IActionResult Login(Admin model)
    {
        if (model.Username == adminUser && model.Password == adminPass)
        {
            HttpContext.Session.SetString("AdminSession", model.Username);
            HttpContext.Session.SetString("AdminName", model.Username);
            return RedirectToAction("Index", "WebSetting", new { area = "Admin" });
        }

        ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
        return View();
    }
    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminSession");
        return RedirectToAction("Login");
    }
}
