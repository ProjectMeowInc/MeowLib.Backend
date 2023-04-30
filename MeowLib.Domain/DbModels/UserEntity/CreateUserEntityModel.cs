namespace MeowLib.Domain.DbModels.UserEntity;

public class CreateUserEntityModel
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}