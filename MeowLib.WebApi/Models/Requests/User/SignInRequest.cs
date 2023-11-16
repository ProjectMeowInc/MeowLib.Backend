namespace MeowLib.WebApi.Models.Requests.User;

public class SignInRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}