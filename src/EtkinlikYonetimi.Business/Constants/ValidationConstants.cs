namespace EtkinlikYonetimi.Business.Constants
{
    /// <summary>
    /// Contains validation constants used throughout the application
    /// </summary>
    public static class ValidationConstants
    {
        /// <summary>
        /// Password validation requirements
        /// </summary>
        public static class Password
        {
            public const int MinLength = 8;
            public const string RequirementsMessage = "Parola en az 8 karakter olmalı, büyük harf, küçük harf ve rakam içermelidir.";
        }

        /// <summary>
        /// File upload validation requirements
        /// </summary>
        public static class FileUpload
        {
            public const int MaxFileSizeInBytes = 2 * 1024 * 1024; // 2MB
            public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        }
    }
}