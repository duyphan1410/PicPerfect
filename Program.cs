using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using CloudinaryDotNet;
using PicPerfect.Helpers;
using PicPerfect.Interface;
using PicPerfect.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure Cloudinary settings from configuration + environment variables
builder.Services.Configure<CloudinarySetting>(
    builder.Configuration.GetSection("CloudinarySetting"));

// Register PhotoServices with IOptions pattern
builder.Services.AddScoped<IPhotoServices, PhotoServices>();

// Configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("localDb")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();