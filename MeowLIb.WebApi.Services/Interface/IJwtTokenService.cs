using MeowLib.Domain.Dto.User;

namespace MeowLIb.WebApi.Services.Interface;

/// <summary>
/// Абстракция сервиса для работы с JWT-токенами.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Токен генерирует JWT-токен для авторизации пользователя.
    /// </summary>
    /// <param name="userData">Данные о пользователе.</param>
    /// <returns>JWT-токен в виде строки.</returns>
    string GenerateToken(UserDto userData);
    
    /// <summary>
    /// Парсит токен и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <returns>Информация о пользователе в случае удачного парсинга, иначе - null</returns>
    Task<UserDto?> ParseToken(string token);
}