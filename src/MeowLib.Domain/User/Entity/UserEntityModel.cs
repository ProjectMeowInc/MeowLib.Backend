using MeowLib.Domain.User.Enums;

namespace MeowLib.Domain.User.Entity;

/// <summary>
/// Класс пользователя, хранящийся в БД.
/// </summary>
public class UserEntityModel
{
    /// <summary>
    /// Id пользователя. PK.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public required string Login { get; set; }

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Токен текущей сессии.
    /// </summary>
    public required string? RefreshToken { get; set; }

    /// <summary>
    /// Количество валюты пользователя.
    /// </summary>
    public required decimal Coins { get; set; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public required UserRolesEnum Role { get; init; } = UserRolesEnum.User;
}