using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория пользователей.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Создаёт пользователя в БД и возвращает его Dto-модель.
    /// </summary>
    /// <param name="createUserData">Данные для создания пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    Task<UserDto> CreateAsync(CreateUserEntityModel createUserData);
    
    /// <summary>
    /// Метод возвращает модель пользователя по его Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>Модель пользователя в случае успешного поиска или null если пользователь не найден</returns>
    Task<UserEntityModel?> GetByIdAsync(int id);
    
    /// <summary>
    /// Метод удаляет пользователя по Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>True - в случае удачного удаления, иначе - false</returns>
    Task<bool> DeleteByIdAsync(int id);
    
    /// <summary>
    /// Метод обновляет информацию о пользователе.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="updateUserData">Данные для обновления.</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="EntityNotFoundException">
    /// Возникает в том случае, если пользователь с заданным Id не найден
    /// </exception>
    Task<Result<UserDto>> UpdateAsync(int id, UpdateUserEntityModel updateUserData);
    
    /// <summary>
    /// Метод проверяет существует ли пользователь с заданным логином.
    /// </summary>
    /// <param name="login">Логин для проверки.</param>
    /// <returns>True - если существует, иначе - false</returns>
    Task<bool> CheckForUserExistAsync(string login);

    /// <summary>
    /// Метод получает пользователя по логину и паролю
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Хеш пароля пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    Task<UserDto?> GetByLoginAndPasswordAsync(string login, string password);

    /// <summary>
    /// Метод получает список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей в формате IQueryable</returns>
    IQueryable<UserEntityModel> GetAll();
}