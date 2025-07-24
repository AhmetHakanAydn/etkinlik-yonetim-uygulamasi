using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if user is logged in (except for Account controller actions)
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();

            if (controllerName != "Account" || (actionName != "Login" && actionName != "Register"))
            {
                if (!SessionHelper.IsUserLoggedIn(HttpContext.Session))
                {
                    context.Result = RedirectToAction("Login", "Account", new { area = "Admin" });
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}