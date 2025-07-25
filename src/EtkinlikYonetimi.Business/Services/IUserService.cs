using EtkinlikYonetimi.Business.DTOs;

namespace EtkinlikYonetimi.Business.Services
{
    /// <summary>
    /// Service interface for user-related operations
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="registerDto">User registration information</param>
        /// <returns>A result indicating success or failure with the created user</returns>
        Task<(bool IsSuccess, string Message, UserDto? User)> RegisterAsync(UserRegisterDto registerDto);

        /// <summary>
        /// Authenticates a user and returns their information
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>A result indicating success or failure with the user information</returns>
        Task<(bool IsSuccess, string Message, UserDto? User)> LoginAsync(UserLoginDto loginDto);

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user DTO if found, null otherwise</returns>
        Task<UserDto?> GetUserByIdAsync(int id);

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <returns>The user DTO if found, null otherwise</returns>
        Task<UserDto?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Updates an existing user's information
        /// </summary>
        /// <param name="userDto">The updated user information</param>
        /// <returns>A result indicating success or failure</returns>
        Task<(bool IsSuccess, string Message)> UpdateUserAsync(UserDto userDto);

        /// <summary>
        /// Checks if an email address is already in use
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
        /// <returns>True if email exists, false otherwise</returns>
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    }
}