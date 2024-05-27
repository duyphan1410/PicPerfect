using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using PicPerfect.Interface;
using PicPerfect.Models;
using PicPerfect.Services;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace PicPerfect.Controllers
{
    public class SignUpController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPhotoServices _photoServices;
        public SignUpController(AppDbContext context, IPhotoServices photoServices)
        {
            _context = context;
            _photoServices = photoServices;
        }

        // GET: SignUp
        public ActionResult SignUp()
        {
            return View();
        }

        // POST: SignUp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(Users user, IFormFile Avatar)
        {
            if (Avatar != null && Avatar.Length > 0)
            {
                // Thực hiện tải lên ảnh lên Cloudinary
                var uploadResult = await _photoServices.AddPhotoAsync(Avatar);
                if (uploadResult.Error == null)
                {
                    // Lưu đường dẫn ảnh vào trường Avatar của mô hình Users
                    user.Avatar = uploadResult.SecureUrl.AbsoluteUri;
                }
                else
                {
                    // Xử lý lỗi tải lên ảnh từ Cloudinary
                    ModelState.AddModelError("", "Error uploading photo: " + uploadResult.Error.Message);
                    return View(user);
                }
            }

            // Kiểm tra tính hợp lệ của ModelState
            if (ModelState.IsValid)
            {
                // Xử lý khi ModelState hợp lệ
                var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Username đã tồn tại.");
                    return View(user);
                }
                if (user.Password != user.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Password and Confirm Password do not match");
                    return View(user);
                }
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "Login");
            }
  
            return View(user);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
