using MeowLib.Domain.DbModels.NotificationEntity.Payload;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.DbModels.NotificationEntity;

public class NotificationEntityModel
{
    public int Id { get; init; }
    public required NotificationTypeEnum Type { get; init; }
    public required BaseNotificationPayload Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsWatched { get; set; }
    public required UserEntityModel User { get; init; }
}