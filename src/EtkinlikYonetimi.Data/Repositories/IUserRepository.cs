using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    }
}