namespace MeowLib.WebApi.Models.Requests.Authorization;

public class UpdateAuthorizationRequest
{
    public required string RefreshToken { get; set; }
}