using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.User.Dto;

namespace MeowLib.Domain.User.Services;

/// <summary>
/// Абстракция сервиса для работы с JWT-токенами.
/// </summary>
public interface IUserTokenService
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
    /// <param name="tokenData">Данные для записи в токен.</param>
    /// <param name="expiredAt">Время истечения токена обновления.</param>
    /// <returns>Токен в виде строки.</returns>
    string GenerateRefreshToken(RefreshTokenDataModel tokenData, DateTime expiredAt);

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
    Task<RefreshTokenDataModel?> ParseRefreshTokenAsync(string token);
}