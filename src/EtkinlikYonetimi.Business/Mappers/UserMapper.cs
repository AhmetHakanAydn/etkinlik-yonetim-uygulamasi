using EtkinlikYonetimi.Business.DTOs;
using EtkinlikYonetimi.Data.Entities;

namespace EtkinlikYonetimi.Business.Mappers
{
    /// <summary>
    /// Provides mapping functionality between User entity and UserDto
    /// </summary>
    public static class UserMapper
    {
        /// <summary>
        /// Maps a User entity to UserDto
        /// </summary>
        /// <param name="user">The user entity to map</param>
        /// <returns>UserDto representation of the user entity</returns>
        /// <exception cref="ArgumentNullException">Thrown when user is null</exception>
        public static UserDto MapToDto(User user)
        {
            ArgumentNullException.ThrowIfNull(user);

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

        /// <summary>
        /// Maps a UserRegisterDto to User entity
        /// </summary>
        /// <param name="registerDto">The registration DTO to map</param>
        /// <param name="encryptedPassword">The encrypted password for the user</param>
        /// <returns>User entity representation of the registration DTO</returns>
        /// <exception cref="ArgumentNullException">Thrown when registerDto is null</exception>
        public static User MapFromRegisterDto(UserRegisterDto registerDto, string encryptedPassword)
        {
            ArgumentNullException.ThrowIfNull(registerDto);

            return new User
            {
                Email = registerDto.Email.ToLowerInvariant(),
                Password = encryptedPassword,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                BirthDate = registerDto.BirthDate,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates a User entity with values from UserDto
        /// </summary>
        /// <param name="user">The user entity to update</param>
        /// <param name="userDto">The DTO containing updated values</param>
        /// <exception cref="ArgumentNullException">Thrown when user or userDto is null</exception>
        public static void UpdateFromDto(User user, UserDto userDto)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(userDto);

            user.Email = userDto.Email.ToLowerInvariant();
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.BirthDate = userDto.BirthDate;
        }
    }
}