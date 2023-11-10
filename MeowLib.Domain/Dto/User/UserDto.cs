using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Dto.User;

/// <summary>
/// DTO для сущности <see cref="UserEntityModel"/>
/// </summary>
public class UserDto
{
    /// <summary>
    /// Id пользователя.
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public required string Login { get; set; } = null!;

    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public required UserRolesEnum Role { get; set; }
}