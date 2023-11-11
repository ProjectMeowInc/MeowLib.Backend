namespace MeowLib.Domain.DbModels.UserEntity;

public class CreateUserEntityModel
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}