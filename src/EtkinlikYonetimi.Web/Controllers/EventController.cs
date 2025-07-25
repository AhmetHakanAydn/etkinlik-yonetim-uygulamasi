using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EtkinlikYonetimi.Web.Controllers
{
    /// <summary>
    /// Controller for displaying event information to the public
    /// </summary>
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private const int LatestEventsCount = 5;

        /// <summary>
        /// Initializes a new instance of the EventController
        /// </summary>
        /// <param name="eventService">The event service</param>
        public EventController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Displays detailed information about a specific event
        /// </summary>
        /// <param name="id">The event ID</param>
        /// <returns>View with event details and related events</returns>
        public async Task<IActionResult> Detail(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            
            if (!IsEventDisplayable(eventDto))
            {
                return NotFound();
            }

            var viewModel = await CreateEventDetailViewModel(eventDto!);
            return View(viewModel);
        }

        /// <summary>
        /// Displays a calendar view of upcoming events
        /// </summary>
        /// <returns>View with upcoming events for calendar display</returns>
        public async Task<IActionResult> Calendar()
        {
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            return View(upcomingEvents);
        }

        /// <summary>
        /// Checks if an event can be displayed to the public
        /// </summary>
        /// <param name="eventDto">The event to check</param>
        /// <returns>True if event can be displayed, false otherwise</returns>
        private static bool IsEventDisplayable(Business.DTOs.EventDto? eventDto)
        {
            return eventDto != null && eventDto.IsActive;
        }

        /// <summary>
        /// Creates a view model for event detail page
        /// </summary>
        /// <param name="eventDto">The main event</param>
        /// <returns>Event detail view model with related events</returns>
        private async Task<EventDetailViewModel> CreateEventDetailViewModel(Business.DTOs.EventDto eventDto)
        {
            var latestEvents = await _eventService.GetLatestEventsAsync(LatestEventsCount);

            return new EventDetailViewModel
            {
                Event = eventDto,
                LatestEvents = latestEvents
            };
        }
    }
}