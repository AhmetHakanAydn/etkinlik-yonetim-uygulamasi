using Microsoft.EntityFrameworkCore;
using EtkinlikYonetimi.Data.Context;
using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    /// <summary>
    /// Repository implementation for User entity operations
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the UserRepository
        /// </summary>
        /// <param name="context">The database context</param>
        public UserRepository(EtkinlikYonetimiDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <returns>The user if found, null otherwise</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Checks if an email address already exists in the system
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
        /// <returns>True if email exists, false otherwise</returns>
        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            var query = _dbSet.Where(u => u.Email == email);
            
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            
            return await query.AnyAsync();
        }
    }
}