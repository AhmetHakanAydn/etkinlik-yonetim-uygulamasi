using EtkinlikYonetimi.Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Business.Services
{
    public interface IEventService
    {
        Task<EventDto?> GetEventByIdAsync(int id);
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<IEnumerable<EventDto>> GetActiveEventsAsync();
        Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
        Task<IEnumerable<EventDto>> GetEventsByUserIdAsync(int userId);
        Task<IEnumerable<EventDto>> GetLatestEventsAsync(int count);
        Task<(bool IsSuccess, string Message, EventDto? Event)> CreateEventAsync(EventDto eventDto);
        Task<(bool IsSuccess, string Message)> UpdateEventAsync(EventDto eventDto);
        Task<(bool IsSuccess, string Message)> DeleteEventAsync(int id);
        Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null);
        Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile);
    }
}