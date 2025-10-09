using System.Data;
using Microsoft.Data.SqlClient;
using TachLayout.Models;

namespace TachLayout.Services
{
    public class WebSettingService
    {
        private readonly string _connectionString;

        public WebSettingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public WebSetting GetWebSetting()
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
            return setting;
        }

        public void UpdateWebSetting(WebSetting model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"UPDATE WebSetting 
                               SET Logo = @Logo, TenSite = @TenSite, DiaChi = @DiaChi, Email = @Email, HotLine = @HotLine";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Logo", model.Logo ?? "");
                cmd.Parameters.AddWithValue("@TenSite", model.TenSite ?? "");
                cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi ?? "");
                cmd.Parameters.AddWithValue("@Email", model.Email ?? "");
                cmd.Parameters.AddWithValue("@HotLine", model.HotLine ?? "");
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
