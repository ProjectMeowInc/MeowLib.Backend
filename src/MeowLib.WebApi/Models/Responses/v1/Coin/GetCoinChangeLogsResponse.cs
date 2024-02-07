namespace MeowLib.WebApi.Models.Responses.v1.Coin;

public class GetCoinChangeLogsResponse
{
    public required IEnumerable<CoinChangeLogModel> Items { get; init; }
}