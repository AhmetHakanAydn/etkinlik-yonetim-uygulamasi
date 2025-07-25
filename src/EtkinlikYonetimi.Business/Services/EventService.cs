using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Data.Entities;
using EtkinlikYonetimi.Data.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Business.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsFolder;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
            
            // Create uploads directory if it doesn't exist
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            return eventEntity != null ? MapToDto(eventEntity) : null;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetActiveEventsAsync()
        {
            var events = await _unitOfWork.Events.GetActiveEventsAsync();
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
        {
            var events = await _unitOfWork.Events.GetUpcomingEventsAsync();
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetEventsByUserIdAsync(int userId)
        {
            var events = await _unitOfWork.Events.GetEventsByUserIdAsync(userId);
            return events.Select(MapToDto);
        }

        public async Task<IEnumerable<EventDto>> GetLatestEventsAsync(int count)
        {
            var events = await _unitOfWork.Events.GetLatestEventsAsync(count);
            return events.Select(MapToDto);
        }

        public async Task<(bool IsSuccess, string Message, EventDto? Event)> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                // Validate dates
                if (eventDto.StartDate >= eventDto.EndDate)
                {
                    return (false, "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.", null);
                }

                // Check if title already exists
                if (await _unitOfWork.Events.IsTitleExistsAsync(eventDto.Title))
                {
                    return (false, "Bu başlık zaten kullanılmaktadır.", null);
                }

                // Handle image upload
                string? imagePath = null;
                if (eventDto.ImageFile != null)
                {
                    var imageUploadResult = await SaveImageAsync(eventDto.ImageFile);
                    if (string.IsNullOrEmpty(imageUploadResult))
                    {
                        return (false, "Resim yükleme sırasında hata oluştu.", null);
                    }
                    imagePath = imageUploadResult;
                }

                // Create event entity
                var eventEntity = new Event
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

                await _unitOfWork.Events.AddAsync(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                var createdEventDto = MapToDto(eventEntity);
                return (true, "Etkinlik başarıyla oluşturuldu.", createdEventDto);
            }
            catch (Exception ex)
            {
                return (false, "Etkinlik oluşturma sırasında bir hata oluştu: " + ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdateEventAsync(EventDto eventDto)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventDto.Id);
                if (eventEntity == null)
                {
                    return (false, "Etkinlik bulunamadı.");
                }

                // Validate dates
                if (eventDto.StartDate >= eventDto.EndDate)
                {
                    return (false, "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");
                }

                // Check if title is being changed and if the new title already exists
                if (eventEntity.Title != eventDto.Title)
                {
                    if (await _unitOfWork.Events.IsTitleExistsAsync(eventDto.Title, eventDto.Id))
                    {
                        return (false, "Bu başlık zaten kullanılmaktadır.");
                    }
                }

                // Handle image upload
                if (eventDto.ImageFile != null)
                {
                    var imageUploadResult = await SaveImageAsync(eventDto.ImageFile);
                    if (string.IsNullOrEmpty(imageUploadResult))
                    {
                        return (false, "Resim yükleme sırasında hata oluştu.");
                    }
                    
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(eventEntity.Image))
                    {
                        DeleteImage(eventEntity.Image);
                    }
                    
                    eventEntity.Image = imageUploadResult;
                }

                // Update event properties
                eventEntity.Title = eventDto.Title;
                eventEntity.StartDate = eventDto.StartDate;
                eventEntity.EndDate = eventDto.EndDate;
                eventEntity.ShortDescription = eventDto.ShortDescription;
                eventEntity.LongDescription = eventDto.LongDescription;
                eventEntity.IsActive = eventDto.IsActive;

                _unitOfWork.Events.Update(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Etkinlik başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return (false, "Güncelleme sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string Message)> DeleteEventAsync(int id)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
                if (eventEntity == null)
                {
                    return (false, "Etkinlik bulunamadı.");
                }

                // Delete associated image
                if (!string.IsNullOrEmpty(eventEntity.Image))
                {
                    DeleteImage(eventEntity.Image);
                }

                _unitOfWork.Events.Remove(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Etkinlik başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return (false, "Silme sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null)
        {
            return await _unitOfWork.Events.IsTitleExistsAsync(title, excludeEventId);
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                // Validate file
                if (imageFile == null || imageFile.Length == 0)
                    return string.Empty;

                // Check file size (2MB max)
                if (imageFile.Length > 2 * 1024 * 1024)
                    return string.Empty;

                // Check file extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return string.Empty;

                // Generate unique filename
                var fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(_uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                return $"/uploads/events/{fileName}";
            }
            catch
            {
                return string.Empty;
            }
        }

        private void DeleteImage(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return;

                var fileName = Path.GetFileName(imagePath);
                var filePath = Path.Combine(_uploadsFolder, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Log error but don't throw
            }
        }

        private static EventDto MapToDto(Event eventEntity)
        {
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
                User = eventEntity.User != null ? new UserDto
                {
                    Id = eventEntity.User.Id,
                    Email = eventEntity.User.Email,
                    FirstName = eventEntity.User.FirstName,
                    LastName = eventEntity.User.LastName,
                    BirthDate = eventEntity.User.BirthDate,
                    CreatedAt = eventEntity.User.CreatedAt
                } : null
            };
        }
    }
}