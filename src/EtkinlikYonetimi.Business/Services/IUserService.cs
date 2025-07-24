using EtkinlikYonetimi.Business.DTOs;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Business.Services
{
    public interface IUserService
    {
        Task<(bool IsSuccess, string Message, UserDto? User)> RegisterAsync(UserRegisterDto registerDto);
        Task<(bool IsSuccess, string Message, UserDto? User)> LoginAsync(UserLoginDto loginDto);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<(bool IsSuccess, string Message)> UpdateUserAsync(UserDto userDto);
        Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null);
    }
}