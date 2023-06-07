using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Models;

namespace MeowLIb.WebApi.Services.Interface;

/// <summary>
/// Абстракция сервиса для работы с JWT-токенами.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Метод генерирует JWT-токен доступа.
    /// </summary>
    /// <param name="userData">Данные о пользователе.</param>
    /// <returns>JWT-токен в виде строки.</returns>
    string GenerateAccessToken(UserDto userData);

    /// <summary>
    /// Метод генерирует JWT-токен обновления.
    /// </summary>
    /// <param name="userLogin">Логин пользователя к которому привязан данный токен.</param>
    /// <param name="expiredAt">Время истечения токена обновления.</param>
    /// <returns></returns>
    string GenerateRefreshToken(string userLogin, DateTime expiredAt);
    
    /// <summary>
    /// Парсит JWT-токен доступа и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <returns>Информация о пользователе в случае удачного парсинга, иначе - null</returns>
    Task<UserDto?> ParseAccessTokenAsync(string token);

    /// <summary>
    /// Метод парсит JWT-токет обновления и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен обновления.</param>
    /// <returns>Информация в токене обновления.</returns>
    Task<AccessTokenDataModel?> ParseRefreshTokenAsync(string token);
}