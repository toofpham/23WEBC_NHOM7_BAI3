using TachLayout.Models;
using System;
using Microsoft.Data.SqlClient;
using TachLayout.Services;
using Microsoft.EntityFrameworkCore;
using TachLayout.Data;

namespace TachLayout
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---- ??ng ký ApplicationDbContext ----
            builder.Services.AddDbContext<TachLayout.Data.AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyConn")));

            builder.Services.AddSingleton<DbService>();
            // ---- ??ng ký WebSettingService v?i scoped lifetime ----
            builder.Services.AddScoped<WebSettingService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}