using MeowLib.Domain.CoinsChangeLog.Enums;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.CoinsChangeLog.Entity;

public class CoinsChangeLogEntityModel
{
    public int Id { get; init; }
    public required decimal Value { get; init; }
    public required CoinsChangeReasonTypeEnum Type { get; init; }
    public required DateTime Date { get; init; }
    public required UserEntityModel User { get; init; }
}