using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Dto.Notification;

public class NotificationDto
{
    public required int Id { get; init; }
    public required NotificationTypeEnum Type { get; init; }
    public required string Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
}