using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TachLayout.Models;

namespace TachLayout.Areas.Admin.ViewComponents
{
   
    public class WebSettingViewComponent : ViewComponent
    {
        private readonly string _connectionString;
        public WebSettingViewComponent(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("QuanLyConn");
        }
        public IViewComponentResult Invoke()
        {
            WebSetting setting = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT TOP 1 * FROM WebSetting";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    setting = new WebSetting
                    {
                        WebSettingID = (int)reader["WebSettingID"],
                        Logo = reader["Logo"].ToString(),
                        TenSite = reader["TenSite"].ToString(),
                        DiaChi = reader["DiaChi"].ToString(),
                        Email = reader["Email"].ToString(),
                        HotLine = reader["HotLine"].ToString()
                    };
                }
            }

            // Gọi partial view `_FooterPartial` và truyền model vào
            return View("~/Views/Shared/_FooterPartial.cshtml", setting);
        }
    }
}
