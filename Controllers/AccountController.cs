using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PicPerfect.DATA;
using PicPerfect.Models;
using PicPerfect.ViewModel;

namespace PicPerfect.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginVM();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM) 
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByNameAsync(loginVM.Username);

            if (user != null)
            {
                //User is found, check password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    //Password correct, sign in
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //Password is incorrect
                TempData["Error"] = "Wrong credentials. Please try again";
                return View(loginVM);
            }
            //User not found
            TempData["Error"] = "Wrong credentials. Please try again";
            return View(loginVM);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var response = new RegisterVM();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerViewModel)
        {
            if (!ModelState.IsValid) return View(registerViewModel);

            var user = await _userManager.FindByNameAsync(registerViewModel.Username);
            if (user != null)
            {
                TempData["Error"] = "This Username is already in use";
                return View(registerViewModel);
            }

            var newUser = new Users()
            {
                Username = registerViewModel.Username
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, registerViewModel.Password);

            if (newUserResponse.Succeeded)
                await _userManager.AddToRoleAsync(newUser, "User");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("Account/Welcome")]
        public async Task<IActionResult> Welcome(int page = 0)
        {
            if (page == 0)
            {
                return View();
            }
            return View();

        }
    }
}
