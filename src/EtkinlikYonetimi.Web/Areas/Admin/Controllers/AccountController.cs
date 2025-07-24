using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Register()
        {
            if (SessionHelper.IsUserLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDto model)
        {
            if (SessionHelper.IsUserLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.RegisterAsync(model);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        public IActionResult Login()
        {
            if (SessionHelper.IsUserLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (SessionHelper.IsUserLoggedIn(HttpContext.Session))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.LoginAsync(model);
            
            if (result.IsSuccess && result.User != null)
            {
                SessionHelper.SetCurrentUser(HttpContext.Session, result.User);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        public IActionResult Logout()
        {
            SessionHelper.ClearCurrentUser(HttpContext.Session);
            return RedirectToAction("Login");
        }
    }
}