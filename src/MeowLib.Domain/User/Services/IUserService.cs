using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Exceptions;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Dto;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.User.Services;

/// <summary>
/// Абстракция сервиса для работы с пользователями.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Метод создаёт нового пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="ApiException">Возникает в случае если логин пользователя занят.</exception>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<Result<UserEntityModel>> SignInAsync(string login, string password);

    /// <summary>
    /// Метод генерирует пару JWT-токенов для авторизации пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <param name="isLongSession">True - RefreshToken будет создан на 30 дней, False - 30 минут.</param>
    /// <returns>Пару JWT-токенов для авторизации.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если авторизационные данные некорректны.</exception>
    /// <exception cref="CreateTokenException">Возникает в случае, если сгенерированные токен уже кому-то принадлежит.</exception>
    Task<Result<(string accessToken, string refreshToken)>> LogIn(string login, string password, bool isLongSession);

    /// <summary>
    /// Метод получает список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    Task<IEnumerable<UserDto>> GetAllAsync();

    /// <summary>
    /// Метод обновляет информацию о пользователе и возвращает его Dto-модель.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="login">Новый логин.</param>
    /// <param name="password">Новый пароль.</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если входные данные были невалидны.</exception>
    Task<Result<UserDto?>> UpdateUser(int id, string? login, string? password);

    /// <summary>
    /// Метод авторизует пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Пару JWT-токенов.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если был введён некорректный токен обновления.</exception>
    Task<Result<(string accessToken, string refreshToken)>> LogInByRefreshTokenAsync(string refreshToken);

    /// <summary>
    /// Метод получает пользователя по Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Найденного пользователя или null</returns>
    Task<UserEntityModel?> GetUserByIdAsync(int userId);
}