namespace MeowLib.WebApi.Models.Requests.v1.Authorization;

public class UpdateAuthorizationRequest
{
    public required string RefreshToken { get; set; }
}