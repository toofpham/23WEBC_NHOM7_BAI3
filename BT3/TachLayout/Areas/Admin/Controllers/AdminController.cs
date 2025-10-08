using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using TachLayout.Filters;
using TachLayout.Models;
using TachLayout.Services;

[Area("Admin")]
[Route("Admin")]
public class AdminController : Controller
{
    private readonly DbService _db;

    public AdminController(DbService db)
    {
        _db = db;
    }



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
        if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
        {
            ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu!";
            return View();
        }

        try
        {
            string sql = @"SELECT TOP 1 UserID, UserName, [Pass]
                           FROM [User]
                           WHERE UserName = @UserName AND [Pass] = @Pass";

            var parameters = new[]
            {
                new SqlParameter("@UserName", model.Username.Trim()),
                new SqlParameter("@Pass", model.Password.Trim())
            };

            DataTable dt = _db.GetDataTable(sql, parameters);

            if (dt == null || dt.Rows.Count == 0)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();
            }

            string userNameFromDb = dt.Rows[0]["UserName"]?.ToString() ?? model.Username;

            HttpContext.Session.SetString("AdminSession", userNameFromDb);
            HttpContext.Session.SetString("AdminName", userNameFromDb);

            return RedirectToAction("Index", "WebSetting", new { area = "Admin" });
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Lỗi SQL: {ex.Message}");
            ViewBag.Error = "Có lỗi hệ thống. Vui lòng thử lại sau.";
            return View();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi đăng nhập: {ex.Message}");
            ViewBag.Error = "Có lỗi không xác định.";
            return View();
        }

        //if (model.Username == adminUser && model.Password == adminPass)
        //{
        //    HttpContext.Session.SetString("AdminSession", model.Username);
        //    HttpContext.Session.SetString("AdminName", model.Username);
        //    return RedirectToAction("Index", "WebSetting", new { area = "Admin" });
        //}

        //ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
        //return View();
    }

[HttpGet("Logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("AdminSession");
        return RedirectToAction("Login");
    }
}
