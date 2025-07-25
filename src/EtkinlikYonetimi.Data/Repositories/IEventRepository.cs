using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    /// <summary>
    /// Repository interface for Event entity with specific event operations
    /// </summary>
    public interface IEventRepository : IRepository<Event>
    {
        /// <summary>
        /// Gets all active events with their user information
        /// </summary>
        /// <returns>Collection of active events</returns>
        Task<IEnumerable<Event>> GetActiveEventsAsync();

        /// <summary>
        /// Gets upcoming events (events that haven't started yet)
        /// </summary>
        /// <returns>Collection of upcoming events</returns>
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();

        /// <summary>
        /// Gets events created by a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of events created by the user</returns>
        Task<IEnumerable<Event>> GetEventsByUserIdAsync(int userId);

        /// <summary>
        /// Gets the latest events up to the specified count
        /// </summary>
        /// <param name="count">Maximum number of events to return</param>
        /// <returns>Collection of latest events</returns>
        Task<IEnumerable<Event>> GetLatestEventsAsync(int count);

        /// <summary>
        /// Checks if an event title already exists in the system
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="excludeEventId">Optional event ID to exclude from the check</param>
        /// <returns>True if title exists, false otherwise</returns>
        Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null);
    }
}