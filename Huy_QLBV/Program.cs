using Huy_QLBV.Models.M0304;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using S0304FirstLoadFlat.Services;
using S0304HTTT.Services;
using S0304NhanVien.Services;
using S0304BangKeThu.Services;
using S0304Report.Services;
using S0304ThongTinDoanhNghiep.Services;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký services
builder.Services.AddSingleton<S0304NhanVienService>();
builder.Services.AddSingleton<S0304HTTTService>();
builder.Services.AddSingleton<S0304FirstLoadFlagService>();

builder.Services.AddSingleton<IS0304BangKeThuService, S0304ReportRepository>();
builder.Services.AddSingleton<IS0304ReportService, S0304ReportService>();
builder.Services.AddSingleton<IS0304ThongTinDoanhNghiepService, S0304ThongTinDoanhNghiepRepository>(); // Singleton để tránh lỗi inject Scoped vào Singleton

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

// Đăng ký DbContext
builder.Services.AddDbContext<M0304Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReportUB"))
);

// Đăng ký session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();        
app.UseAuthorization();

// Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=C0304BangKeThu}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=C0304BangKeThu}/{action=Index}/{id?}"
);

app.Run();
