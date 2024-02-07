using MeowLib.Domain.CoinsChangeLog.Enums;

namespace MeowLib.Domain.CoinsChangeLog.Dto;

public class CoinsChangeLogDto
{
    public required int Id { get; init; }
    public required decimal Value { get; init; }
    public required CoinsChangeReasonTypeEnum Type { get; init; }
    public required DateTime Date { get; init; }
}