using MeowLib.Domain.User.Enums;

namespace MeowLib.WebApi.Models.Responses.v1.User;

public class UserModel
{
    public required int Id { get; init; }
    public required string Login { get; init; }
    public required UserRolesEnum Role { get; init; }
}