namespace MeowLib.Domain.Requests.User;

public class UpdateUserRequest
{
    public string? Login { get; set; }
    public string? Password { get; set; }
}