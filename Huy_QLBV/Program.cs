using S0304HTTT.Services;
using Huy_QLBV.Models.M0304; // namespace Models của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using S0304NhanVien.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<S0304NhanVienService>();
builder.Services.AddSingleton<S0304HTTTService>();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =
            Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Đăng ký DbContext D0304Context
builder.Services.AddDbContext<M0304Context>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ReportUB")
    )
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

builder.Services.AddSession();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=C0304BangKeThu}/{action=Index}/{id?}"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=C0304BangKeThu}/{action=Index}/{id?}"
);

app.Run();
