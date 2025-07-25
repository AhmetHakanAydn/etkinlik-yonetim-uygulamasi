using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace EtkinlikYonetimi.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing events in the admin area
    /// </summary>
    public class EventManagementController : BaseController
    {
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the EventManagementController
        /// </summary>
        /// <param name="eventService">The event service</param>
        public EventManagementController(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Displays the list of events for the current user
        /// </summary>
        /// <returns>View with user's events</returns>
        public async Task<IActionResult> Index()
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            var events = await _eventService.GetEventsByUserIdAsync(currentUser.Id);
            return View(events);
        }

        /// <summary>
        /// Displays the create event form
        /// </summary>
        /// <returns>View with new event model</returns>
        public IActionResult Create()
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            var model = CreateNewEventModel(currentUser.Id);
            return View(model);
        }

        /// <summary>
        /// Handles the creation of a new event
        /// </summary>
        /// <param name="model">The event data</param>
        /// <returns>Redirect to index on success, or view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventDto model)
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            model.UserId = currentUser.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _eventService.CreateEventAsync(model);
            return HandleServiceResult((result.IsSuccess, result.Message), () => RedirectToAction("Index"));
        }

        /// <summary>
        /// Displays the edit event form
        /// </summary>
        /// <param name="id">The event ID</param>
        /// <returns>View with event data for editing</returns>
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            var eventDto = await GetEventForCurrentUser(id, currentUser.Id);
            if (eventDto == null)
            {
                return NotFound();
            }

            return View(eventDto);
        }

        /// <summary>
        /// Handles the update of an existing event
        /// </summary>
        /// <param name="model">The updated event data</param>
        /// <returns>Redirect to index on success, or view with errors</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventDto model)
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            // Ensure user can only edit their own events
            var existingEvent = await GetEventForCurrentUser(model.Id, currentUser.Id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            model.UserId = currentUser.Id;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _eventService.UpdateEventAsync(model);
            return HandleServiceResult(result, () => RedirectToAction("Index"));
        }

        /// <summary>
        /// Handles the deletion of an event
        /// </summary>
        /// <param name="id">The event ID</param>
        /// <returns>Redirect to index with success or error message</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = GetCurrentUserOrRedirect();
            if (currentUser == null)
            {
                return RedirectToLoginAction();
            }

            var eventDto = await GetEventForCurrentUser(id, currentUser.Id);
            if (eventDto == null)
            {
                return NotFound();
            }

            var result = await _eventService.DeleteEventAsync(id);
            
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets the current user from session or returns null
        /// </summary>
        /// <returns>Current user DTO or null</returns>
        private UserDto? GetCurrentUserOrRedirect()
        {
            return SessionHelper.GetCurrentUser(HttpContext.Session);
        }

        /// <summary>
        /// Creates a redirect action to the login page
        /// </summary>
        /// <returns>Redirect action result</returns>
        private IActionResult RedirectToLoginAction()
        {
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Creates a new event model with default values
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>New event DTO with defaults</returns>
        private static EventDto CreateNewEventModel(int userId)
        {
            var tomorrow = DateTime.Now.AddDays(1);
            return new EventDto
            {
                UserId = userId,
                StartDate = tomorrow,
                EndDate = tomorrow.AddHours(2),
                IsActive = true
            };
        }

        /// <summary>
        /// Gets an event for the current user, ensuring ownership
        /// </summary>
        /// <param name="eventId">The event ID</param>
        /// <param name="userId">The user ID</param>
        /// <returns>Event DTO if found and owned by user, null otherwise</returns>
        private async Task<EventDto?> GetEventForCurrentUser(int eventId, int userId)
        {
            var eventDto = await _eventService.GetEventByIdAsync(eventId);
            return eventDto?.UserId == userId ? eventDto : null;
        }

        /// <summary>
        /// Handles service result and returns appropriate action result
        /// </summary>
        /// <param name="result">The service result</param>
        /// <param name="successAction">Action to execute on success</param>
        /// <returns>Action result based on service result</returns>
        private IActionResult HandleServiceResult((bool IsSuccess, string Message) result, Func<IActionResult> successAction)
        {
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message;
                return successAction();
            }

            ModelState.AddModelError("", result.Message);
            return View();
        }
    }
}