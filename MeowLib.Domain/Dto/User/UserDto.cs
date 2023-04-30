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
    public int Id { get; set; }
    
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; } = null!;
    
    /// <summary>
    /// Роль пользователя.
    /// </summary>
    public UserRolesEnum Role { get; set; }
}