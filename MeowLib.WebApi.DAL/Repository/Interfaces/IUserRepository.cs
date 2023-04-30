
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IUserRepository
{
    Task<UserDto> CreateAsync(CreateUserEntityModel createUserData);
    Task<UserDto?> GetByIdAsync(int id);
    Task<bool> DeleteByIdAsync(int id);
    Task<UserDto> UpdateAsync(int id, UpdateUserEntityModel updateUserData);
    Task<bool> CheckForUserExistAsync(string login);

    /// <summary>
    /// Метод получает пользователя по логину и паролю
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Хеш пароля пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    Task<UserDto?> GetByLoginAndPasswordAsync(string login, string password);
}