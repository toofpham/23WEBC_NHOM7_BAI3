using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TachLayout.Models;
using TachLayout.Data;

namespace TachLayout.Areas.Admin.ViewComponents
{
   
    public class WebSettingViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public WebSettingViewComponent(AppDbContext context)
        {
            _context = context; 
        }
        public IViewComponentResult Invoke()
        {
            // ---- Lấy record đầu tiên ----
            var setting = _context.WebSettings.FirstOrDefault();

            // ---- Gọi partial view _FooterPartial.cshtml và truyền model ----
            return View("~/Views/Shared/_FooterPartial.cshtml", setting);
        }
    }
}
