using Microsoft.AspNetCore.Http;
using System.Text.Json;
using EtkinlikYonetimi.Business.DTOs;

namespace EtkinlikYonetimi.Web.Helpers
{
    public static class SessionHelper
    {
        private const string USER_SESSION_KEY = "CurrentUser";

        public static void SetCurrentUser(ISession session, UserDto user)
        {
            session.SetString(USER_SESSION_KEY, JsonSerializer.Serialize(user));
        }

        public static UserDto? GetCurrentUser(ISession session)
        {
            var userJson = session.GetString(USER_SESSION_KEY);
            if (string.IsNullOrEmpty(userJson))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<UserDto>(userJson);
            }
            catch
            {
                return null;
            }
        }

        public static void ClearCurrentUser(ISession session)
        {
            session.Remove(USER_SESSION_KEY);
        }

        public static bool IsUserLoggedIn(ISession session)
        {
            return GetCurrentUser(session) != null;
        }
    }
}