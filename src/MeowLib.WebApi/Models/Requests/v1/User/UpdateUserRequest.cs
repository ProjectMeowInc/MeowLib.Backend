using MeowLib.Domain.User.Enums;

namespace MeowLib.WebApi.Models.Requests.v1.User;

public class UpdateUserRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public UserRolesEnum? Role { get; set; }
}