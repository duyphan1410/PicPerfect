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
using PicPerfect.Services;

namespace PicPerfect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPhotoServices _photoServices;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, IPhotoServices photoServices, AppDbContext context)
        {
            _logger = logger;
            _photoServices = photoServices;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("username");
            if (!string.IsNullOrEmpty(username))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    ViewBag.FullName = user.Fullname;
                    ViewBag.Username = user.Username;

                    // Lấy danh sách ảnh của user
                    var userImages = await _context.Images
                        .Where(i => i.UserId == user.UserId)
                        .OrderByDescending(i => i.UploadDatetime)
                        .Take(10) // Lấy 10 ảnh gần nhất
                        .ToListAsync();

                    ViewBag.UserImages = userImages;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Không có file nào được chọn" });
                }

                // Kiểm tra định dạng file
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Định dạng file không được hỗ trợ. Chỉ chấp nhận: jpg, jpeg, png, gif" });
                }

                // Kiểm tra kích thước file (tối đa 10MB)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return Json(new { success = false, message = "File quá lớn. Kích thước tối đa là 10MB" });
                }

                // Upload ảnh lên Cloudinary
                var uploadResult = await _photoServices.AddPhotoAsync(file);

                if (uploadResult.Error != null)
                {
                    return Json(new { success = false, message = uploadResult.Error.Message });
                }

                // Lưu thông tin ảnh vào database
                var username = HttpContext.Session.GetString("username");
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để lưu ảnh" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var image = new Images
                {

                    ImagePath = uploadResult.SecureUrl.ToString(),
                    UploadDatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    UserId = user.UserId
                };

                _context.Images.Add(image);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Upload ảnh thành công",
                    imageUrl = uploadResult.SecureUrl.ToString(),
                    imageId = image.ImageId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upload ảnh");
                return Json(new { success = false, message = "Có lỗi xảy ra khi upload ảnh: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUserImages()
        {
            try
            {
                var username = HttpContext.Session.GetString("username");
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy thông tin người dùng" });
                }

                var images = await _context.Images
                    .Where(i => i.UserId == user.UserId)
                    .OrderByDescending(i => i.UploadDatetime)
                    .Take(10)
                    .Select(i => new
                    {
                        i.ImageId,
                        i.ImagePath,
                        i.ImageName,
                        i.UploadDatetime
                    })
                    .ToListAsync();

                return Json(new { success = true, images });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách ảnh");
                return Json(new { success = false, message = "Có lỗi xảy ra khi lấy danh sách ảnh" });
            }
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
