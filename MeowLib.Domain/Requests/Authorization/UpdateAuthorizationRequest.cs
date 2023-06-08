namespace MeowLib.Domain.Requests.Authorization;

public class UpdateAuthorizationRequest
{
    public required string RefreshToken { get; set; }
}