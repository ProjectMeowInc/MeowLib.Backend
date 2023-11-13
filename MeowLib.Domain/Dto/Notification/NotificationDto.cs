using MeowLib.Domain.DbModels.NotificationEntity.Payload;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Dto.Notification;

public class NotificationDto
{
    public required int Id { get; init; }
    public required NotificationTypeEnum Type { get; init; }
    public required BaseNotificationPayload Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
}