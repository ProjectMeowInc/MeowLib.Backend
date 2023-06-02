using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;

namespace MeowLIb.WebApi.Services.Interface;

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
    /// Метод создаёт JWT-токег для авторизации пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>JWT-токен для авторизации.</returns>
    /// <exception cref="ApiException">Возникает в случае если указан неверный логин или пароль</exception>
    Task<Result<string>> LogIn(string login, string password);

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
}