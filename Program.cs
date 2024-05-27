using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using CloudinaryDotNet;
using PicPerfect.Helpers;
using PicPerfect.Interface;
using PicPerfect.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


builder.Services.AddScoped<IPhotoServices, PhotoServices>();
builder.Services.Configure<CloudinarySetting>(builder.Configuration.GetSection("CloudinarySetting"));

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(
    builder.Configuration.GetConnectionString("localDb")));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication
    ();


app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

ConfigureCloudinary(builder);
void ConfigureCloudinary(WebApplicationBuilder builder)
{
    // Thay thế 'YOUR_CLOUD_NAME', 'YOUR_API_KEY', và 'YOUR_API_SECRET' bằng thông tin tài khoản Cloudinary của bạn
    Account account = new Account(
        "dwvt3snha",
        "222353231937235",
        "sb83WZRaSQheptlLB4bcR7QDI1I");

    Cloudinary cloudinary = new Cloudinary(account);

    // Cấu hình các thiết lập khác cho Cloudinary nếu cần
    // Ví dụ:
    // cloudinary.Secure = true;

    // Đăng ký Cloudinary vào dịch vụ của ứng dụng
    builder.Services.AddSingleton(cloudinary);
}