using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Web.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null || !eventDto.IsActive)
            {
                return NotFound();
            }

            var latestEvents = await _eventService.GetLatestEventsAsync(5);

            var viewModel = new EventDetailViewModel
            {
                Event = eventDto,
                LatestEvents = latestEvents
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Calendar()
        {
            var upcomingEvents = await _eventService.GetUpcomingEventsAsync();
            return View(upcomingEvents);
        }
    }
}