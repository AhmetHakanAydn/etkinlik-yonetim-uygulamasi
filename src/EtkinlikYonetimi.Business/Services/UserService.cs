using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Helpers;
using EtkinlikYonetimi.Business.Services;
using EtkinlikYonetimi.Data.Entities;
using EtkinlikYonetimi.Data.Repositories;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EtkinlikYonetimi.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<(bool IsSuccess, string Message, UserDto? User)> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                // Validate password requirements
                if (!IsPasswordValid(registerDto.Password))
                {
                    return (false, "Parola en az 8 karakter olmalı, büyük harf, küçük harf ve rakam içermelidir.", null);
                }

                // Check if email already exists
                if (await _unitOfWork.Users.IsEmailExistsAsync(registerDto.Email))
                {
                    return (false, "Bu email adresi zaten kullanılmaktadır.", null);
                }

                // Create user entity
                var user = new User
                {
                    Email = registerDto.Email.ToLower(),
                    Password = EncryptionHelper.EncryptPassword(registerDto.Password),
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    BirthDate = registerDto.BirthDate,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = MapToDto(user);
                return (true, "Kullanıcı başarıyla kaydedildi.", userDto);
            }
            catch (Exception ex)
            {
                return (false, "Kullanıcı kaydı sırasında bir hata oluştu: " + ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message, UserDto? User)> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email.ToLower());
                
                if (user == null)
                {
                    return (false, "Email adresi bulunamadı.", null);
                }

                if (!EncryptionHelper.VerifyPassword(loginDto.Password, user.Password))
                {
                    return (false, "Parola hatalı.", null);
                }

                var userDto = MapToDto(user);
                return (true, "Giriş başarılı.", userDto);
            }
            catch (Exception ex)
            {
                return (false, "Giriş sırasında bir hata oluştu: " + ex.Message, null);
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email.ToLower());
            return user != null ? MapToDto(user) : null;
        }

        public async Task<(bool IsSuccess, string Message)> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
                if (user == null)
                {
                    return (false, "Kullanıcı bulunamadı.");
                }

                // Check if email is being changed and if the new email already exists
                if (user.Email != userDto.Email.ToLower())
                {
                    if (await _unitOfWork.Users.IsEmailExistsAsync(userDto.Email, userDto.Id))
                    {
                        return (false, "Bu email adresi zaten kullanılmaktadır.");
                    }
                }

                // Update user properties
                user.Email = userDto.Email.ToLower();
                user.FirstName = userDto.FirstName;
                user.LastName = userDto.LastName;
                user.BirthDate = userDto.BirthDate;

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return (true, "Kullanıcı bilgileri başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return (false, "Güncelleme sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await _unitOfWork.Users.IsEmailExistsAsync(email.ToLower(), excludeUserId);
        }

        private static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            // Check for at least one uppercase, one lowercase, and one digit
            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasDigit = Regex.IsMatch(password, @"\d");

            return hasUpperCase && hasLowerCase && hasDigit;
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                CreatedAt = user.CreatedAt
            };
        }
    }
}