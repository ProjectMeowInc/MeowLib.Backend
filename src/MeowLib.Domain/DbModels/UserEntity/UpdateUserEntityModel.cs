using MeowLib.Domain.Enums;

namespace MeowLib.Domain.DbModels.UserEntity;

public class UpdateUserEntityModel
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public UserRolesEnum? Role { get; set; }
}