using EtkinlikYonetimi.Business.DTOs;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetimi.Business.Services
{
    /// <summary>
    /// Service interface for event-related operations
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Gets an event by its ID
        /// </summary>
        /// <param name="id">The event ID</param>
        /// <returns>The event DTO if found, null otherwise</returns>
        Task<EventDto?> GetEventByIdAsync(int id);

        /// <summary>
        /// Gets all events in the system
        /// </summary>
        /// <returns>Collection of all events</returns>
        Task<IEnumerable<EventDto>> GetAllEventsAsync();

        /// <summary>
        /// Gets all active events
        /// </summary>
        /// <returns>Collection of active events</returns>
        Task<IEnumerable<EventDto>> GetActiveEventsAsync();

        /// <summary>
        /// Gets upcoming events (events that haven't started yet)
        /// </summary>
        /// <returns>Collection of upcoming events</returns>
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();

        /// <summary>
        /// Gets events created by a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of events created by the user</returns>
        Task<IEnumerable<EventDto>> GetEventsByUserIdAsync(int userId);

        /// <summary>
        /// Gets the latest events up to the specified count
        /// </summary>
        /// <param name="count">Maximum number of events to return</param>
        /// <returns>Collection of latest events</returns>
        Task<IEnumerable<EventDto>> GetLatestEventsAsync(int count);

        /// <summary>
        /// Creates a new event in the system
        /// </summary>
        /// <param name="eventDto">Event information</param>
        /// <returns>A result indicating success or failure with the created event</returns>
        Task<(bool IsSuccess, string Message, EventDto? Event)> CreateEventAsync(EventDto eventDto);

        /// <summary>
        /// Updates an existing event
        /// </summary>
        /// <param name="eventDto">Updated event information</param>
        /// <returns>A result indicating success or failure</returns>
        Task<(bool IsSuccess, string Message)> UpdateEventAsync(EventDto eventDto);

        /// <summary>
        /// Deletes an event from the system
        /// </summary>
        /// <param name="id">The ID of the event to delete</param>
        /// <returns>A result indicating success or failure</returns>
        Task<(bool IsSuccess, string Message)> DeleteEventAsync(int id);

        /// <summary>
        /// Checks if an event title already exists
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="excludeEventId">Optional event ID to exclude from the check</param>
        /// <returns>True if title exists, false otherwise</returns>
        Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null);

        /// <summary>
        /// Saves an uploaded image file to the server
        /// </summary>
        /// <param name="imageFile">The image file to save</param>
        /// <returns>The relative path to the saved image, or empty string if failed</returns>
        Task<string> SaveImageAsync(IFormFile imageFile);
    }
}