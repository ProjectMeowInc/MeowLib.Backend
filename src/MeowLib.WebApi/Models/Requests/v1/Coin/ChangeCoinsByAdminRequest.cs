namespace MeowLib.WebApi.Models.Requests.v1.Coin;

public class ChangeCoinsByAdminRequest
{
    public required int UserId { get; init; }
    public required decimal UpdateCoins { get; init; }
}