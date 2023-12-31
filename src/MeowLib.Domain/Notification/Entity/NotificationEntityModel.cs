using MeowLib.Domain.Notification.Enums;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.Notification.Entity;

public class NotificationEntityModel
{
    public int Id { get; init; }
    public required NotificationTypeEnum Type { get; init; }
    public required string Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required bool IsWatched { get; set; }
    public required UserEntityModel User { get; init; }
}