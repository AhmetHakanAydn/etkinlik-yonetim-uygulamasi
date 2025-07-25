using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EtkinlikYonetimi.Web.Models;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Models.ViewModels;

namespace EtkinlikYonetimi.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEventService _eventService;

    public HomeController(ILogger<HomeController> logger, IEventService eventService)
    {
        _logger = logger;
        _eventService = eventService;
    }

    public async Task<IActionResult> Index()
    {
        var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
        
        var viewModel = new EventListViewModel
        {
            Events = upcomingEvents,
            PageTitle = "Etkinlik Yönetim Uygulaması",
            PageDescription = "Yaklaşan etkinlikleri keşfedin ve takip edin."
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
