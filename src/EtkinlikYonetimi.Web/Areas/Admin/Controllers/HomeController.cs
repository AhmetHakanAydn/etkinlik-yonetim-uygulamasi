using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            ViewBag.CurrentUser = currentUser;
            return View();
        }
    }
}