using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    public class ProfileController : BaseController
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(currentUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserDto model)
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure the user can only update their own profile
            model.Id = currentUser.Id;

            var result = await _userService.UpdateUserAsync(model);
            
            if (result.IsSuccess)
            {
                // Update session with new user data
                var updatedUser = await _userService.GetUserByIdAsync(currentUser.Id);
                if (updatedUser != null)
                {
                    SessionHelper.SetCurrentUser(HttpContext.Session, updatedUser);
                }

                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }
    }
}