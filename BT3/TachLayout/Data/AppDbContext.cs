using Microsoft.EntityFrameworkCore;
using TachLayout.Models;

namespace TachLayout.Data
{
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Dbset đại diện cho bảng Websetting trong DB
        public DbSet<WebSetting> WebSettings { get; set; }
    }
}
