using Microsoft.EntityFrameworkCore;
using EtkinlikYonetimi.Data.Context;
using EtkinlikYonetimi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Data.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(EtkinlikYonetimiDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Event>> GetActiveEventsAsync()
        {
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.IsActive)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.IsActive && e.StartDate > now)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

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