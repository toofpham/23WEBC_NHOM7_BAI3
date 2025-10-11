using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace TachLayout.Filters
{
    public class AdminAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            var username = context.HttpContext.Session.GetString("Username");

            // Nếu chưa đăng nhập hoặc không phải admin => chuyển hướng đến trang đăng nhập
            if (string.IsNullOrEmpty(username) || !string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
