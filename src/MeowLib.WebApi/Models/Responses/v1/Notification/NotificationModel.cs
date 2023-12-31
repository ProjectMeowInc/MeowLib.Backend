using MeowLib.Domain.Notification.Enums;

namespace MeowLib.WebApi.Models.Responses.v1.Notification;

public class NotificationModel
{
    public required int Id { get; init; }
    public required NotificationTypeEnum Type { get; init; }
    public required string Payload { get; init; }
    public required DateTime CreatedAt { get; init; }
}