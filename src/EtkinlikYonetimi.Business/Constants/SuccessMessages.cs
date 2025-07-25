namespace EtkinlikYonetimi.Business.Constants
{
    /// <summary>
    /// Contains success message constants used throughout the application
    /// </summary>
    public static class SuccessMessages
    {
        /// <summary>
        /// User-related success messages
        /// </summary>
        public static class User
        {
            public const string RegistrationSuccessful = "Kullanıcı başarıyla kaydedildi.";
            public const string LoginSuccessful = "Giriş başarılı.";
            public const string UpdateSuccessful = "Kullanıcı bilgileri başarıyla güncellendi.";
        }

        /// <summary>
        /// Event-related success messages
        /// </summary>
        public static class Event
        {
            public const string CreationSuccessful = "Etkinlik başarıyla oluşturuldu.";
            public const string UpdateSuccessful = "Etkinlik başarıyla güncellendi.";
            public const string DeleteSuccessful = "Etkinlik başarıyla silindi.";
        }
    }
}