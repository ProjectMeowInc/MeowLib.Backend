using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Result;

namespace MeowLib.Services.Interface;

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
    Task<Result<UserDto>> SignInAsync(string login, string password);

    /// <summary>
    /// Метод генерирует пару JWT-токенов для авторизации пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <param name="isLongSession">True - RefreshToken будет создан на 30 дней, False - 30 минут.</param>
    /// <returns>Пару JWT-токенов для авторизации.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если авторизационные данные некорректны.</exception>
    /// <exception cref="CreateTokenException">Возникает в случае, если сгенерированные токен уже кому-то принадлежит.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
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
    /// <param name="updateData">Данные для обновления.</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если входные данные были невалидны.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в том случае, если пользователь с заданным Id не найден.</exception>
    Task<Result<UserDto>> UpdateUser(int id, UpdateUserEntityModel updateData);

    /// <summary>
    /// Метод авторизует пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Пару JWT-токенов.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если был введён некорректный токен обновления.</exception>
    Task<Result<(string accessToken, string refreshToken)>> LogInByRefreshTokenAsync(string refreshToken);
}