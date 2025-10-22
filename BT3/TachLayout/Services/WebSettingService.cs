using System.Data;
using Microsoft.Data.SqlClient;
using TachLayout.Models;
using TachLayout.Data;

namespace TachLayout.Services
{
    public class WebSettingService
    {
        private readonly AppDbContext _context;

        public WebSettingService(AppDbContext context)
        {
            _context = context;
        }

        // ---- Lấy setting ----
        public WebSetting GetWebSetting()
        {
            return _context.WebSettings.FirstOrDefault();
        }

        // ---- Cập nhật setting ----
        public void UpdateWebSetting(WebSetting model)
        {
            // ---- Sử dụng Linq FirstOrDefault ----
            var existing = _context.WebSettings.FirstOrDefault();
            if(existing != model)
            {
                // ---- Cập nhật các trường ----
                existing.TenSite = model.TenSite;
                existing.Logo = model.Logo;
                existing.DiaChi = model.DiaChi;
                existing.Email = model.Email;
                existing.HotLine = model.HotLine;

                // ---- Lưu thay đổi vào DB ----
                _context.SaveChanges();
            }
        }
    }
}
