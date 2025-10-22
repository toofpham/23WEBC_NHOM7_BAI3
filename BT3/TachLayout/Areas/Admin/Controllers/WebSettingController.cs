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
        private readonly IWebHostEnvironment _env;
        private readonly WebSettingService _webSettingService;

        public WebSettingController(WebSettingService webSettingService, IWebHostEnvironment env)
        {
            // ---- // Inject service (service đã có DbContext) ----
            _webSettingService = webSettingService;
            _env = env;
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
            // ---- Kiểm tra nếu có file được gửi lên ----
            if(LogoFile != null && LogoFile.Length > 0)
            {
                // ---- Tạo đường dẫn thư mục lưu file: wwwroot/uploads/logo ----
                var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "logo");

                // ---- Tạo thư mục nếu chưa tồn tại ----
                if(!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // ---- Tạo file duy nhất bằng Guid ----
                var fileExtension = Path.GetExtension(LogoFile.FileName); // -- Lấy phần mở rộng vd: .jpg, .png --
                var uniqueFileName = Guid.NewGuid().ToString() + fileExtension; // -- Tên file duy nhất --
                var filePath = Path.Combine(uploadPath, uniqueFileName);

                // ---- Lưu file vào thư mục  đồng bộ ----
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    LogoFile.CopyTo(stream);
                }

                // ---- Lưu đường dẫn tương đối vào model (ví dụ: /uploads/logo/123.jpg) ----
                model.Logo = $"/uploads/logo/{uniqueFileName}";

                // Thông báo thành công
                ViewBag.Message = "Upload logo thành công!";
            }
            else if(LogoFile != null && LogoFile.Length == 0)
            {
                // ---- Thông báo nếu file rỗng ----
                ViewBag.Message = "File logo không hợp lệ. Vui lòng chọn file hợp lệ.";
            }
            else
            {
                // ---- Nếu không có file, giữ nguyên logo cũ (không ghi đè model.Logo) ----
                ViewBag.Message = "Không có logo mới được chọn.";
            }

            // ---- Cập nhật thông tin WebSetting vào database ----
            _webSettingService.UpdateWebSetting(model);

            // ---- Chuyển hướng về trang WebSetting ----
            return RedirectToAction("WebSetting");




            //if (LogoFile != null && LogoFile.Length > 0)
            //{
            //    // Tạo đường dẫn lưu file
            //    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "logo");
            //    if (!Directory.Exists(uploadsFolder))
            //        Directory.CreateDirectory(uploadsFolder);

            //    // Đặt tên file duy nhất
            //    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
            //    string filePath = Path.Combine(uploadsFolder, fileName);

            //    // Lưu file vào thư mục wwwroot/uploads/logo
            //    using (var stream = new FileStream(filePath, FileMode.Create))
            //    {
            //        LogoFile.CopyTo(stream);
            //    }

            //    // Lưu đường dẫn (ví dụ: /uploads/logo/abc.jpg)
            //    model.Logo = "/uploads/logo/" + fileName;
            //}

            //_webSettingService.UpdateWebSetting(model);
            //return RedirectToAction("WebSetting");


        }
    }
}
