namespace MeowLib.Domain.Shared.Models;

/// <summary>
/// Класс описывает данные, хранящиеся в JWT-токене для обновления.
/// </summary>
public class RefreshTokenDataModel
{
    /// <summary>
    /// Логин пользователя-владельца токена.
    /// </summary>
    public required string Login { get; init; }

    /// <summary>
    /// Долгая ли сессия?
    /// </summary>
    public required bool IsLongSession { get; init; }
}