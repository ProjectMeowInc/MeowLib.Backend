namespace MeowLib.Domain.Responses.User;

public class LogInResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public LogInResponse(string accessToken, string refreshToken = "Not workings")
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}