using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Result;

namespace MeowLib.DAL.Repository.Interfaces;

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
    Task<Result<UserDto?>> UpdateAsync(int id, UpdateUserEntityModel updateUserData);

    /// <summary>
    /// Метод проверяет существует ли пользователь с заданным логином.
    /// </summary>
    /// <param name="login">Логин для проверки.</param>
    /// <returns>True - если существует, иначе - false</returns>
    Task<bool> CheckForUserExistAsync(string login);

    /// <summary>
    /// Метод получает пользователя по логину и паролю.
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

    /// <summary>
    /// Метод получает пользователя по логину.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <returns>Модель пользователя или null если он не был найден.</returns>
    Task<UserEntityModel?> GetByLoginAsync(string login);

    /// <summary>
    /// Метод получает пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Найденного пользователя или null</returns>
    Task<UserEntityModel?> GetByRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Метод изменяет токена обновления пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="newRefreshToken">Новый токен обновления.</param>
    /// <returns>Ошибку, если она есть.</returns>
    Task<Result> UpdateRefreshTokenAsync(string login, string newRefreshToken);
}