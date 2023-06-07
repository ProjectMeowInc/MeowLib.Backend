namespace MeowLib.Domain.Requests.User;

public class LogInRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required bool LongSession { get; set; }
}