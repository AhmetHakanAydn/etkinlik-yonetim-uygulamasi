using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    /// <summary>
    /// Repository interface for User entity with specific user operations
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <returns>The user if found, null otherwise</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Checks if an email address already exists in the system
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
        /// <returns>True if email exists, false otherwise</returns>
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    }
}