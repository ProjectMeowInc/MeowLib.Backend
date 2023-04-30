namespace MeowLib.Domain.Requests.User;

public class SignInRequest
{
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;
}