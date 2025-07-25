using EtkinlikYonetimi.Business.Constants;
using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Mappers;
using EtkinlikYonetimi.Business.Validators;
using EtkinlikYonetimi.Data.Entities;
using EtkinlikYonetimi.Data.Repositories;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetimi.Business.Services
{
    /// <summary>
    /// Service for managing event-related operations
    /// </summary>
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadsFolder;

        /// <summary>
        /// Initializes a new instance of the EventService class
        /// </summary>
        /// <param name="unitOfWork">The unit of work for data access</param>
        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "events");
            
            // Create uploads directory if it doesn't exist
            EnsureUploadsDirectoryExists();
        }

        /// <summary>
        /// Ensures the uploads directory exists, creating it if necessary
        /// </summary>
        private void EnsureUploadsDirectoryExists()
        {
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        /// <summary>
        /// Gets an event by its ID
        /// </summary>
        /// <param name="id">The event ID</param>
        /// <returns>The event DTO if found, null otherwise</returns>
        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
            return eventEntity != null ? EventMapper.MapToDto(eventEntity) : null;
        }

        /// <summary>
        /// Gets all events in the system
        /// </summary>
        /// <returns>Collection of all events</returns>
        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Events.GetAllAsync();
            return events.Select(EventMapper.MapToDto);
        }

        /// <summary>
        /// Gets all active events
        /// </summary>
        /// <returns>Collection of active events</returns>
        public async Task<IEnumerable<EventDto>> GetActiveEventsAsync()
        {
            var events = await _unitOfWork.Events.GetActiveEventsAsync();
            return events.Select(EventMapper.MapToDto);
        }

        /// <summary>
        /// Gets upcoming events (events that haven't started yet)
        /// </summary>
        /// <returns>Collection of upcoming events</returns>
        public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
        {
            var events = await _unitOfWork.Events.GetUpcomingEventsAsync();
            return events.Select(EventMapper.MapToDto);
        }

        /// <summary>
        /// Gets events created by a specific user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Collection of events created by the user</returns>
        public async Task<IEnumerable<EventDto>> GetEventsByUserIdAsync(int userId)
        {
            var events = await _unitOfWork.Events.GetEventsByUserIdAsync(userId);
            return events.Select(EventMapper.MapToDto);
        }

        /// <summary>
        /// Gets the latest events up to the specified count
        /// </summary>
        /// <param name="count">Maximum number of events to return</param>
        /// <returns>Collection of latest events</returns>
        public async Task<IEnumerable<EventDto>> GetLatestEventsAsync(int count)
        {
            var events = await _unitOfWork.Events.GetLatestEventsAsync(count);
            return events.Select(EventMapper.MapToDto);
        }

        /// <summary>
        /// Creates a new event in the system
        /// </summary>
        /// <param name="eventDto">Event information</param>
        /// <returns>A result indicating success or failure with the created event</returns>
        public async Task<(bool IsSuccess, string Message, EventDto? Event)> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                // Validate event data
                var validationResult = await ValidateEventForCreation(eventDto);
                if (!validationResult.IsValid)
                {
                    return (false, validationResult.ErrorMessage, null);
                }

                // Handle image upload if provided
                var imagePath = await ProcessImageUpload(eventDto.ImageFile);
                if (eventDto.ImageFile != null && string.IsNullOrEmpty(imagePath))
                {
                    return (false, ErrorMessages.Event.ImageUploadError, null);
                }

                // Create and save the event
                var eventEntity = await CreateAndSaveEvent(eventDto, imagePath);
                var createdEventDto = EventMapper.MapToDto(eventEntity);
                
                return (true, SuccessMessages.Event.CreationSuccessful, createdEventDto);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.Event.CreationError + ex.Message, null);
            }
        }

        /// <summary>
        /// Validates event data for creation
        /// </summary>
        /// <param name="eventDto">The event DTO to validate</param>
        /// <returns>Validation result</returns>
        private async Task<(bool IsValid, string ErrorMessage)> ValidateEventForCreation(EventDto eventDto)
        {
            // Validate date range
            if (!ValidationHelper.IsDateRangeValid(eventDto.StartDate, eventDto.EndDate))
            {
                return (false, ErrorMessages.Event.InvalidDateRange);
            }

            // Check if title already exists
            if (await _unitOfWork.Events.IsTitleExistsAsync(eventDto.Title))
            {
                return (false, ErrorMessages.Event.TitleAlreadyExists);
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Processes image upload and returns the image path
        /// </summary>
        /// <param name="imageFile">The image file to upload</param>
        /// <returns>The image path if successful, null or empty string otherwise</returns>
        private async Task<string?> ProcessImageUpload(IFormFile? imageFile)
        {
            if (imageFile == null) return null;
            
            return await SaveImageAsync(imageFile);
        }

        /// <summary>
        /// Creates and saves a new event entity
        /// </summary>
        /// <param name="eventDto">The event DTO</param>
        /// <param name="imagePath">The image path</param>
        /// <returns>The created event entity</returns>
        private async Task<Event> CreateAndSaveEvent(EventDto eventDto, string? imagePath)
        {
            var eventEntity = EventMapper.MapFromDto(eventDto, imagePath);

            await _unitOfWork.Events.AddAsync(eventEntity);
            await _unitOfWork.SaveChangesAsync();

            return eventEntity;
        }

        /// <summary>
        /// Updates an existing event
        /// </summary>
        /// <param name="eventDto">Updated event information</param>
        /// <returns>A result indicating success or failure</returns>
        public async Task<(bool IsSuccess, string Message)> UpdateEventAsync(EventDto eventDto)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventDto.Id);
                if (eventEntity == null)
                {
                    return (false, ErrorMessages.Event.EventNotFound);
                }

                // Validate event data for update
                var validationResult = await ValidateEventForUpdate(eventDto, eventEntity);
                if (!validationResult.IsValid)
                {
                    return (false, validationResult.ErrorMessage);
                }

                // Handle image upload if provided
                var newImagePath = await HandleImageUpdateIfNeeded(eventDto.ImageFile, eventEntity.Image);
                if (eventDto.ImageFile != null && string.IsNullOrEmpty(newImagePath))
                {
                    return (false, ErrorMessages.Event.ImageUploadError);
                }

                // Update event entity and save
                EventMapper.UpdateFromDto(eventEntity, eventDto, newImagePath);
                _unitOfWork.Events.Update(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                return (true, SuccessMessages.Event.UpdateSuccessful);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.Event.UpdateError + ex.Message);
            }
        }

        /// <summary>
        /// Validates event data for update
        /// </summary>
        /// <param name="eventDto">The event DTO to validate</param>
        /// <param name="existingEvent">The existing event entity</param>
        /// <returns>Validation result</returns>
        private async Task<(bool IsValid, string ErrorMessage)> ValidateEventForUpdate(EventDto eventDto, Event existingEvent)
        {
            // Validate date range
            if (!ValidationHelper.IsDateRangeValid(eventDto.StartDate, eventDto.EndDate))
            {
                return (false, ErrorMessages.Event.InvalidDateRange);
            }

            // Check if title is being changed and if the new title already exists
            if (existingEvent.Title != eventDto.Title)
            {
                if (await _unitOfWork.Events.IsTitleExistsAsync(eventDto.Title, eventDto.Id))
                {
                    return (false, ErrorMessages.Event.TitleAlreadyExists);
                }
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Handles image update if a new image file is provided
        /// </summary>
        /// <param name="newImageFile">The new image file</param>
        /// <param name="currentImagePath">The current image path</param>
        /// <returns>The new image path if updated, null otherwise</returns>
        private async Task<string?> HandleImageUpdateIfNeeded(IFormFile? newImageFile, string? currentImagePath)
        {
            if (newImageFile == null) return null;

            var newImagePath = await SaveImageAsync(newImageFile);
            if (!string.IsNullOrEmpty(newImagePath) && !string.IsNullOrEmpty(currentImagePath))
            {
                // Delete old image if new image was uploaded successfully
                DeleteImage(currentImagePath);
            }

            return newImagePath;
        }

        /// <summary>
        /// Deletes an event from the system
        /// </summary>
        /// <param name="id">The ID of the event to delete</param>
        /// <returns>A result indicating success or failure</returns>
        public async Task<(bool IsSuccess, string Message)> DeleteEventAsync(int id)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
                if (eventEntity == null)
                {
                    return (false, ErrorMessages.Event.EventNotFound);
                }

                // Delete associated image if exists
                if (!string.IsNullOrEmpty(eventEntity.Image))
                {
                    DeleteImage(eventEntity.Image);
                }

                _unitOfWork.Events.Remove(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                return (true, SuccessMessages.Event.DeleteSuccessful);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.Event.DeleteError + ex.Message);
            }
        }

        /// <summary>
        /// Checks if an event title already exists
        /// </summary>
        /// <param name="title">The title to check</param>
        /// <param name="excludeEventId">Optional event ID to exclude from the check</param>
        /// <returns>True if title exists, false otherwise</returns>
        public async Task<bool> IsTitleExistsAsync(string title, int? excludeEventId = null)
        {
            return await _unitOfWork.Events.IsTitleExistsAsync(title, excludeEventId);
        }

        /// <summary>
        /// Saves an uploaded image file to the server
        /// </summary>
        /// <param name="imageFile">The image file to save</param>
        /// <returns>The relative path to the saved image, or empty string if failed</returns>
        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            try
            {
                // Validate the image file
                if (!ValidationHelper.IsValidImageFile(imageFile))
                    return string.Empty;

                // Generate unique filename and save
                var fileName = ValidationHelper.GenerateUniqueFileName(imageFile.FileName);
                var filePath = Path.Combine(_uploadsFolder, fileName);

                // Save file to disk
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

        /// <summary>
        /// Deletes an image file from the server
        /// </summary>
        /// <param name="imagePath">The path to the image to delete</param>
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
                // Log error but don't throw - file deletion failure shouldn't break the application
            }
        }
    }
}