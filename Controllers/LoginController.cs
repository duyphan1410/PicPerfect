using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicPerfect.DATA;
using PicPerfect.Models;

namespace PicPerfect.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        public LoginController(AppDbContext context)
        {
            _context = context;
        }
        public ActionResult Login()
        {
            return View(); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null) // dang nhap thanh cong
            {
                HttpContext.Session.SetString("username", user.Username); // set session la (key-value) 
                ViewBag.FullName = user.Fullname;
                ViewBag.Username = user.Username;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View();
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
