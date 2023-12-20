namespace MeowLib.WebApi.Models.Responses.v1.User;

public class LogInResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}