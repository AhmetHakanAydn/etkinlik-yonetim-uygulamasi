namespace EtkinlikYonetimi.Business.Constants
{
    /// <summary>
    /// Contains error message constants used throughout the application
    /// </summary>
    public static class ErrorMessages
    {
        /// <summary>
        /// User-related error messages
        /// </summary>
        public static class User
        {
            public const string EmailAlreadyExists = "Bu email adresi zaten kullanılmaktadır.";
            public const string EmailNotFound = "Email adresi bulunamadı.";
            public const string IncorrectPassword = "Parola hatalı.";
            public const string UserNotFound = "Kullanıcı bulunamadı.";
            public const string RegistrationError = "Kullanıcı kaydı sırasında bir hata oluştu: ";
            public const string LoginError = "Giriş sırasında bir hata oluştu: ";
            public const string UpdateError = "Güncelleme sırasında bir hata oluştu: ";
        }

        /// <summary>
        /// Event-related error messages
        /// </summary>
        public static class Event
        {
            public const string InvalidDateRange = "Bitiş tarihi başlangıç tarihinden sonra olmalıdır.";
            public const string TitleAlreadyExists = "Bu başlık zaten kullanılmaktadır.";
            public const string EventNotFound = "Etkinlik bulunamadı.";
            public const string ImageUploadError = "Resim yükleme sırasında hata oluştu.";
            public const string CreationError = "Etkinlik oluşturma sırasında bir hata oluştu: ";
            public const string UpdateError = "Güncelleme sırasında bir hata oluştu: ";
            public const string DeleteError = "Silme sırasında bir hata oluştu: ";
        }
    }
}