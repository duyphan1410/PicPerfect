using Microsoft.AspNetCore.Mvc;
using PicPerfect.Models;
using System.Diagnostics;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using PicPerfect.Interface;
using PicPerfect.DATA.Enum;
using System.Web;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;

namespace PicPerfect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoServices _photoServices;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, IPhotoServices photoServices,AppDbContext context)
        {
            _logger = logger;
            _photoServices = photoServices;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("username"));
            if(user != null)
            {
                ViewBag.FullName = user.Fullname;
                ViewBag.Username = user.Username;
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SaveImage(Images img,IFormFile ImagePath)
        {

            string username = HttpContext.Session.GetString("username");
            if (string.IsNullOrEmpty(username))
            {
                RedirectToAction("Login", "Login"); // Hoặc chuyển hướng người dùng đến trang đăng nhập
            }
            if (ImagePath == null || ImagePath.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            // Đẩy hình ảnh lên Cloudinary
            var uploadResult = await _photoServices.AddPhotoAsync(ImagePath);
            if (uploadResult.Error != null)
            {
                // Xử lý lỗi khi tải lên Cloudinary
                return StatusCode(500, "Error uploading image to Cloudinary.");
            }

            // Tạo một đối tượng mới để lưu thông tin hình ảnh vào cơ sở dữ liệu
            var image = new Images
            {
                UploadDatetime = DateTime.Now.ToString("dd/MM/yyyy"), // Cập nhật thời gian tải lên
                ImagePath = uploadResult.SecureUrl.AbsoluteUri // Lưu URL của hình ảnh từ Cloudinary vào trường ImagePath
            };

            // Thêm đối tượng hình ảnh vào cơ sở dữ liệu
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            return Ok("Image saved successfully.");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
};
