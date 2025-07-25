using EtkinlikYonetimi.Business.Constants;
using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Business.Helpers;
using EtkinlikYonetimi.Business.Mappers;
using EtkinlikYonetimi.Business.Validators;
using EtkinlikYonetimi.Data.Entities;
using EtkinlikYonetimi.Data.Repositories;

namespace EtkinlikYonetimi.Business.Services
{
    /// <summary>
    /// Service for managing user-related operations
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the UserService class
        /// </summary>
        /// <param name="unitOfWork">The unit of work for data access</param>
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="registerDto">User registration information</param>
        /// <returns>A result indicating success or failure with the created user</returns>
        public async Task<(bool IsSuccess, string Message, UserDto? User)> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                // Validate password requirements
                if (!ValidationHelper.IsPasswordValid(registerDto.Password))
                {
                    return (false, ValidationConstants.Password.RequirementsMessage, null);
                }

                // Check if email already exists
                if (await IsEmailAlreadyInUse(registerDto.Email))
                {
                    return (false, ErrorMessages.User.EmailAlreadyExists, null);
                }

                // Create and save the user
                var user = await CreateUserFromRegisterDto(registerDto);
                var userDto = UserMapper.MapToDto(user);
                
                return (true, SuccessMessages.User.RegistrationSuccessful, userDto);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.User.RegistrationError + ex.Message, null);
            }
        }

        /// <summary>
        /// Creates a new user entity from registration DTO and saves it to the database
        /// </summary>
        /// <param name="registerDto">The registration DTO</param>
        /// <returns>The created user entity</returns>
        private async Task<User> CreateUserFromRegisterDto(UserRegisterDto registerDto)
        {
            var encryptedPassword = EncryptionHelper.EncryptPassword(registerDto.Password);
            var user = UserMapper.MapFromRegisterDto(registerDto, encryptedPassword);

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Checks if an email is already in use by another user
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
        /// <returns>True if email is already in use, false otherwise</returns>
        private async Task<bool> IsEmailAlreadyInUse(string email, int? excludeUserId = null)
        {
            return await _unitOfWork.Users.IsEmailExistsAsync(email.ToLowerInvariant(), excludeUserId);
        }

        /// <summary>
        /// Authenticates a user and returns their information
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>A result indicating success or failure with the user information</returns>
        public async Task<(bool IsSuccess, string Message, UserDto? User)> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email.ToLowerInvariant());
                
                if (user == null)
                {
                    return (false, ErrorMessages.User.EmailNotFound, null);
                }

                if (!EncryptionHelper.VerifyPassword(loginDto.Password, user.Password))
                {
                    return (false, ErrorMessages.User.IncorrectPassword, null);
                }

                var userDto = UserMapper.MapToDto(user);
                return (true, SuccessMessages.User.LoginSuccessful, userDto);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.User.LoginError + ex.Message, null);
            }
        }

        /// <summary>
        /// Gets a user by their ID
        /// </summary>
        /// <param name="id">The user ID</param>
        /// <returns>The user DTO if found, null otherwise</returns>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user != null ? UserMapper.MapToDto(user) : null;
        }

        /// <summary>
        /// Gets a user by their email address
        /// </summary>
        /// <param name="email">The user's email address</param>
        /// <returns>The user DTO if found, null otherwise</returns>
        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email.ToLowerInvariant());
            return user != null ? UserMapper.MapToDto(user) : null;
        }

        /// <summary>
        /// Updates an existing user's information
        /// </summary>
        /// <param name="userDto">The updated user information</param>
        /// <returns>A result indicating success or failure</returns>
        public async Task<(bool IsSuccess, string Message)> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id);
                if (user == null)
                {
                    return (false, ErrorMessages.User.UserNotFound);
                }

                // Check if email is being changed and if the new email already exists
                if (await IsEmailBeingChangedToExistingEmail(user, userDto))
                {
                    return (false, ErrorMessages.User.EmailAlreadyExists);
                }

                // Update user properties using mapper
                UserMapper.UpdateFromDto(user, userDto);

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return (true, SuccessMessages.User.UpdateSuccessful);
            }
            catch (Exception ex)
            {
                return (false, ErrorMessages.User.UpdateError + ex.Message);
            }
        }

        /// <summary>
        /// Checks if the email is being changed to an email that already exists
        /// </summary>
        /// <param name="currentUser">The current user entity</param>
        /// <param name="updatedUserDto">The updated user DTO</param>
        /// <returns>True if email is being changed to an existing email, false otherwise</returns>
        private async Task<bool> IsEmailBeingChangedToExistingEmail(User currentUser, UserDto updatedUserDto)
        {
            var newEmailLower = updatedUserDto.Email.ToLowerInvariant();
            var currentEmailLower = currentUser.Email.ToLowerInvariant();

            if (currentEmailLower != newEmailLower)
            {
                return await _unitOfWork.Users.IsEmailExistsAsync(newEmailLower, updatedUserDto.Id);
            }

            return false;
        }

        /// <summary>
        /// Checks if an email address is already in use
        /// </summary>
        /// <param name="email">The email address to check</param>
        /// <param name="excludeUserId">Optional user ID to exclude from the check</param>
        /// <returns>True if email exists, false otherwise</returns>
        public async Task<bool> IsEmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await _unitOfWork.Users.IsEmailExistsAsync(email.ToLowerInvariant(), excludeUserId);
        }
    }
}