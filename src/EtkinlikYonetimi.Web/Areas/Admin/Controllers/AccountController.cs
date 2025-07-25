using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for handling user authentication in the admin area
    /// </summary>
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="userService">The user service</param>
        public AccountController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Displays the user registration form
        /// </summary>
        /// <returns>View for user registration</returns>
        public IActionResult Register()
        {
            if (IsUserLoggedIn())
            {
                return RedirectToHomePage();
            }
            return View();
        }

        /// <summary>
        /// Handles user registration
        /// </summary>
        /// <param name="model">User registration data</param>
        /// <returns>Redirect to login on success, or view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterDto model)
        {
            if (IsUserLoggedIn())
            {
                return RedirectToHomePage();
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

        /// <summary>
        /// Displays the user login form
        /// </summary>
        /// <returns>View for user login</returns>
        public IActionResult Login()
        {
            if (IsUserLoggedIn())
            {
                return RedirectToHomePage();
            }
            return View();
        }

        /// <summary>
        /// Handles user login
        /// </summary>
        /// <param name="model">User login credentials</param>
        /// <returns>Redirect to home on success, or view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginDto model)
        {
            if (IsUserLoggedIn())
            {
                return RedirectToHomePage();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _userService.LoginAsync(model);
            
            if (result.IsSuccess && result.User != null)
            {
                SessionHelper.SetCurrentUser(HttpContext.Session, result.User);
                return RedirectToHomePage();
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        /// <summary>
        /// Handles user logout
        /// </summary>
        /// <returns>Redirect to login page</returns>
        public IActionResult Logout()
        {
            SessionHelper.ClearCurrentUser(HttpContext.Session);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Checks if a user is currently logged in
        /// </summary>
        /// <returns>True if user is logged in, false otherwise</returns>
        private bool IsUserLoggedIn()
        {
            return SessionHelper.IsUserLoggedIn(HttpContext.Session);
        }

        /// <summary>
        /// Creates a redirect action to the home page
        /// </summary>
        /// <returns>Redirect action result</returns>
        private IActionResult RedirectToHomePage()
        {
            return RedirectToAction("Index", "Home");
        }
    }
}