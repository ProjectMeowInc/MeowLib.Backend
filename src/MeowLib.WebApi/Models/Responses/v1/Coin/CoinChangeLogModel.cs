using MeowLib.Domain.CoinsChangeLog.Enums;

namespace MeowLib.WebApi.Models.Responses.v1.Coin;

public class CoinChangeLogModel
{
    public required int Id { get; init; }
    public required decimal Value { get; init; }
    public required CoinsChangeReasonTypeEnum Reason { get; init; }
    public required DateTime Date { get; init; }
}