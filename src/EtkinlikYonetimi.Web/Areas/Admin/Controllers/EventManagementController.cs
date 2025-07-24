using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    public class EventManagementController : BaseController
    {
        private readonly IEventService _eventService;

        public EventManagementController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var events = await _eventService.GetEventsByUserIdAsync(currentUser.Id);
            return View(events);
        }

        public IActionResult Create()
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new EventDto
            {
                UserId = currentUser.Id,
                StartDate = DateTime.Now.AddDays(1),
                EndDate = DateTime.Now.AddDays(1).AddHours(2)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventDto model)
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            model.UserId = currentUser.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _eventService.CreateEventAsync(model);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null || eventDto.UserId != currentUser.Id)
            {
                return NotFound();
            }

            return View(eventDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventDto model)
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Ensure user can only edit their own events
            var existingEvent = await _eventService.GetEventByIdAsync(model.Id);
            if (existingEvent == null || existingEvent.UserId != currentUser.Id)
            {
                return NotFound();
            }

            model.UserId = currentUser.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _eventService.UpdateEventAsync(model);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = SessionHelper.GetCurrentUser(HttpContext.Session);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null || eventDto.UserId != currentUser.Id)
            {
                return NotFound();
            }

            var result = await _eventService.DeleteEventAsync(id);
            
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Index");
        }
    }
}