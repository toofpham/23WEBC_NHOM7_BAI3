using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TachLayout.Filters;
using TachLayout.Models;
using TachLayout.Services;

namespace TachLayout.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    [AdminAuthorize]
    public class WebSettingController : Controller
    {
        private readonly WebSettingService _webSettingService;

        public WebSettingController(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("QuanLyConn");
            _webSettingService = new WebSettingService(connectionString);
        }

        [HttpGet("WebSetting")]
        public IActionResult WebSetting()
        {
            ViewData["Title"] = "Admin - WebSetting";
            var setting = _webSettingService.GetWebSetting();
            return View(setting);
        }

        [HttpGet("EditWebSetting")]
        public IActionResult EditWebSetting()
        {
            var setting = _webSettingService.GetWebSetting();
            return View(setting);
        }

        [HttpPost("EditWebSetting")]
        public IActionResult EditWebSetting(WebSetting model, IFormFile? LogoFile)
        {
            if (LogoFile != null && LogoFile.Length > 0)
            {
                // Tạo đường dẫn lưu file
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logo");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Đặt tên file duy nhất
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file vào thư mục wwwroot/uploads/logo
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }

                // Lưu đường dẫn (ví dụ: /uploads/logo/abc.jpg)
                model.Logo = "/uploads/logo/" + fileName;
            }

            _webSettingService.UpdateWebSetting(model);
            return RedirectToAction("WebSetting");
        }
    }
}
