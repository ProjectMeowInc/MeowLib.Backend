using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.DbModels.CoinsChangeLogEntity;

public class CoinsChangeLogEntityModel
{
    public int Id { get; init; }
    public required decimal Value { get; init; }
    public required MoneyChangeReasonTypeEnum Type { get; init; }
    public required DateTime Date { get; init; }
    public required UserEntityModel User { get; init; }
}