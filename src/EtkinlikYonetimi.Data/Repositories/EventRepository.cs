using Microsoft.EntityFrameworkCore;
using EtkinlikYonetimi.Data.Context;
using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    /// <summary>
    /// Repository implementation for Event entity operations
    /// </summary>
    public class EventRepository : Repository<Event>, IEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the EventRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public EventRepository(EtkinlikYonetimiDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets all active events with their user information
        /// </summary>
        /// <returns>Collection of active events</returns>
        public async Task<IEnumerable<Event>> GetActiveEventsAsync()
        {
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets upcoming events (events that haven't started yet)
        /// </summary>
        /// <returns>Collection of upcoming events</returns>
        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.IsActive && e.StartDate > now)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets events created by a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of events created by the user</returns>
        public async Task<IEnumerable<Event>> GetEventsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the latest events up to the specified count
        /// </summary>
        /// <param name="count">Maximum number of events to return</param>
        /// <returns>Collection of latest events</returns>
        public async Task<IEnumerable<Event>> GetLatestEventsAsync(int count)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.IsActive && e.StartDate > now)
                .OrderByDescending(e => e.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        /// <summary>
        /// Checks if an event title already exists in the system
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="excludeEventId">Optional event ID to exclude from the check</param>
        /// <returns>True if title exists, false otherwise</returns>
        public async Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null)
        {
            var query = _dbSet.Where(e => e.Title == title);
            
            if (excludeEventId.HasValue)
            {
                query = query.Where(e => e.Id != excludeEventId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}