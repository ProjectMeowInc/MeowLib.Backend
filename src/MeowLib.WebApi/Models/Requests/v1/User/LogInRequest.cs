namespace MeowLib.WebApi.Models.Requests.v1.User;

public class LogInRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required bool IsLongSession { get; set; }
}