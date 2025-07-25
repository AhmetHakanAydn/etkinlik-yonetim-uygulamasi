using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Business.Mappers
{
    /// <summary>
    /// Provides mapping functionality between Event entity and EventDto
    /// </summary>
    public static class EventMapper
    {
        /// <summary>
        /// Maps an Event entity to EventDto
        /// </summary>
        /// <param name="eventEntity">The event entity to map</param>
        /// <returns>EventDto representation of the event entity</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventEntity is null</exception>
        public static EventDto MapToDto(Event eventEntity)
        {
            ArgumentNullException.ThrowIfNull(eventEntity);

            return new EventDto
            {
                Id = eventEntity.Id,
                Title = eventEntity.Title,
                StartDate = eventEntity.StartDate,
                EndDate = eventEntity.EndDate,
                Image = eventEntity.Image,
                ShortDescription = eventEntity.ShortDescription,
                LongDescription = eventEntity.LongDescription,
                IsActive = eventEntity.IsActive,
                UserId = eventEntity.UserId,
                CreatedAt = eventEntity.CreatedAt,
                User = eventEntity.User != null ? UserMapper.MapToDto(eventEntity.User) : null
            };
        }

        /// <summary>
        /// Maps an EventDto to Event entity for creation
        /// </summary>
        /// <param name="eventDto">The event DTO to map</param>
        /// <param name="imagePath">The image path for the event</param>
        /// <returns>Event entity representation of the event DTO</returns>
        /// <exception cref="ArgumentNullException">Thrown when eventDto is null</exception>
        public static Event MapFromDto(EventDto eventDto, string? imagePath = null)
        {
            ArgumentNullException.ThrowIfNull(eventDto);

            return new Event
            {
                Title = eventDto.Title,
                StartDate = eventDto.StartDate,
                EndDate = eventDto.EndDate,
                Image = imagePath,
                ShortDescription = eventDto.ShortDescription,
                LongDescription = eventDto.LongDescription,
                IsActive = eventDto.IsActive,
                UserId = eventDto.UserId,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates an Event entity with values from EventDto
        /// </summary>
        /// <param name="eventEntity">The event entity to update</param>
        /// <param name="eventDto">The DTO containing updated values</param>
        /// <param name="newImagePath">Optional new image path if image was updated</param>
        /// <exception cref="ArgumentNullException">Thrown when eventEntity or eventDto is null</exception>
        public static void UpdateFromDto(Event eventEntity, EventDto eventDto, string? newImagePath = null)
        {
            ArgumentNullException.ThrowIfNull(eventEntity);
            ArgumentNullException.ThrowIfNull(eventDto);

            eventEntity.Title = eventDto.Title;
            eventEntity.StartDate = eventDto.StartDate;
            eventEntity.EndDate = eventDto.EndDate;
            eventEntity.ShortDescription = eventDto.ShortDescription;
            eventEntity.LongDescription = eventDto.LongDescription;
            eventEntity.IsActive = eventDto.IsActive;

            if (newImagePath != null)
            {
                eventEntity.Image = newImagePath;
            }
        }
    }
}