using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EtkinlikYonetimi.Web.Models;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Models.ViewModels;

namespace EtkinlikYonetimi.Web.Controllers
{
    /// <summary>
    /// Main controller for the public-facing pages
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the HomeController
        /// </summary>
        /// <param name="logger">The logger instance</param>
        /// <param name="eventService">The event service</param>
        public HomeController(ILogger<HomeController> logger, IEventService eventService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Displays the home page with upcoming events
        /// </summary>
        /// <returns>View with upcoming events</returns>
        public async Task<IActionResult> Index()
        {
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            
            var viewModel = CreateEventListViewModel(upcomingEvents);
            return View(viewModel);
        }

        /// <summary>
        /// Displays the privacy policy page
        /// </summary>
        /// <returns>Privacy policy view</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page
        /// </summary>
        /// <returns>Error view with request information</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel 
            { 
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
            };
            
            return View(errorViewModel);
        }

        /// <summary>
        /// Creates an event list view model with page metadata
        /// </summary>
        /// <param name="events">The events to display</param>
        /// <returns>Configured event list view model</returns>
        private static EventListViewModel CreateEventListViewModel(IEnumerable<Business.DTOs.EventDto> events)
        {
            return new EventListViewModel
            {
                Events = events,
                PageTitle = "Etkinlik Yönetim Uygulaması",
                PageDescription = "Yaklaşan etkinlikleri keşfedin ve takip edin."
            };
        }
    }
}
