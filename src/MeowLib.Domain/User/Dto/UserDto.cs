using MeowLib.Domain.User.Entity;
using MeowLib.Domain.User.Enums;

namespace MeowLib.Domain.User.Dto;

/// <summary>
/// DTO для сущности <see cref="UserEntityModel" />
/// </summary>
public class UserDto
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public required string Login { get; init; }

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public required UserRolesEnum Role { get; init; }
}