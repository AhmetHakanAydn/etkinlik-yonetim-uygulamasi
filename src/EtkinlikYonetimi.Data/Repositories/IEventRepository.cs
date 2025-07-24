using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync();
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetEventsByUserIdAsync(int userId);
        Task<IEnumerable<Event>> GetLatestEventsAsync(int count);
        Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null);
    }
}