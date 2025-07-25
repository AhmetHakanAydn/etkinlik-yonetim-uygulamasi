using System.Text.RegularExpressions;
using EtkinlikYonetimi.Business.Constants;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetimi.Business.Validators
{
    /// <summary>
    /// Provides validation functionality for common business rules
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Validates if a password meets the security requirements
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>True if password meets requirements, false otherwise</returns>
        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < ValidationConstants.Password.MinLength)
                return false;

            // Check for at least one uppercase, one lowercase, and one digit
            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasDigit = Regex.IsMatch(password, @"\d");

            return hasUpperCase && hasLowerCase && hasDigit;
        }

        /// <summary>
        /// Validates if a date range is valid (end date after start date)
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>True if date range is valid, false otherwise</returns>
        public static bool IsDateRangeValid(DateTime startDate, DateTime endDate)
        {
            return endDate > startDate;
        }

        /// <summary>
        /// Validates if an uploaded file is a valid image
        /// </summary>
        /// <param name="imageFile">The uploaded file to validate</param>
        /// <returns>True if file is a valid image, false otherwise</returns>
        public static bool IsValidImageFile(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return false;

            // Check file size
            if (imageFile.Length > ValidationConstants.FileUpload.MaxFileSizeInBytes)
                return false;

            // Check file extension
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            return ValidationConstants.FileUpload.AllowedImageExtensions.Contains(fileExtension);
        }

        /// <summary>
        /// Generates a unique filename for an uploaded file
        /// </summary>
        /// <param name="originalFileName">The original filename</param>
        /// <returns>A unique filename with the same extension</returns>
        public static string GenerateUniqueFileName(string originalFileName)
        {
            var fileExtension = Path.GetExtension(originalFileName);
            return Guid.NewGuid().ToString() + fileExtension;
        }
    }
}