namespace MeowLib.Domain.Requests.User;

public class LogInRequest
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}