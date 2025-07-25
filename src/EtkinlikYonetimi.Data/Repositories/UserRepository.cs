using Microsoft.EntityFrameworkCore;
using EtkinlikYonetimi.Data.Context;
using EtkinlikYonetimi.Data.Entities;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(EtkinlikYonetimiDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

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