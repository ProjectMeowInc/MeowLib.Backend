namespace MeowLib.Domain.Responses.User;

public class LogInResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}