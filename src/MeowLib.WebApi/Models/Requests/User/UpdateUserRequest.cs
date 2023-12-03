using MeowLib.Domain.Enums;

namespace MeowLib.WebApi.Models.Requests.User;

public class UpdateUserRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
    public UserRolesEnum? Role { get; set; }
}