using Microsoft.AspNetCore.Http;
using System.Text.Json;
using EtkinlikYonetimi.Business.DTOs;

namespace EtkinlikYonetimi.Web.Helpers
{
    /// <summary>
    /// Helper class for managing user session data
    /// </summary>
    public static class SessionHelper
    {
        private const string UserSessionKey = "CurrentUser";

        /// <summary>
        /// Sets the current user in the session
        /// </summary>
        /// <param name="session">The HTTP session</param>
        /// <param name="user">The user data to store</param>
        /// <exception cref="ArgumentNullException">Thrown when session or user is null</exception>
        public static void SetCurrentUser(ISession session, UserDto user)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentNullException.ThrowIfNull(user);

            var userJson = JsonSerializer.Serialize(user, GetJsonSerializerOptions());
            session.SetString(UserSessionKey, userJson);
        }

        /// <summary>
        /// Gets the current user from the session
        /// </summary>
        /// <param name="session">The HTTP session</param>
        /// <returns>The current user if found, null otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when session is null</exception>
        public static UserDto? GetCurrentUser(ISession session)
        {
            ArgumentNullException.ThrowIfNull(session);

            var userJson = session.GetString(UserSessionKey);
            if (string.IsNullOrEmpty(userJson))
            {
                return null;
            }

            return TryDeserializeUser(userJson);
        }

        /// <summary>
        /// Clears the current user from the session
        /// </summary>
        /// <param name="session">The HTTP session</param>
        /// <exception cref="ArgumentNullException">Thrown when session is null</exception>
        public static void ClearCurrentUser(ISession session)
        {
            ArgumentNullException.ThrowIfNull(session);
            session.Remove(UserSessionKey);
        }

        /// <summary>
        /// Checks if a user is currently logged in
        /// </summary>
        /// <param name="session">The HTTP session</param>
        /// <returns>True if user is logged in, false otherwise</returns>
        /// <exception cref="ArgumentNullException">Thrown when session is null</exception>
        public static bool IsUserLoggedIn(ISession session)
        {
            return GetCurrentUser(session) != null;
        }

        /// <summary>
        /// Attempts to deserialize user JSON data
        /// </summary>
        /// <param name="userJson">The JSON string to deserialize</param>
        /// <returns>UserDto if deserialization succeeds, null otherwise</returns>
        private static UserDto? TryDeserializeUser(string userJson)
        {
            try
            {
                return JsonSerializer.Deserialize<UserDto>(userJson, GetJsonSerializerOptions());
            }
            catch (JsonException)
            {
                // Log the error in a real application
                return null;
            }
        }

        /// <summary>
        /// Gets JSON serializer options for consistent serialization
        /// </summary>
        /// <returns>JsonSerializerOptions with consistent settings</returns>
        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }
    }
}