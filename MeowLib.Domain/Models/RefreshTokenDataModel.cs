namespace MeowLib.Domain.Models;

/// <summary>
/// Класс описывает данные, хранящиеся в JWT-токене для обновления.
/// </summary>
public class RefreshTokenDataModel
{
    /// <summary>
    /// Логин пользователя-владельца токена.
    /// </summary>
    public required string Login { get; set; }
    
    /// <summary>
    /// Долгая ли сессия?
    /// </summary>
    public required bool IsLongSession { get; set; }
}