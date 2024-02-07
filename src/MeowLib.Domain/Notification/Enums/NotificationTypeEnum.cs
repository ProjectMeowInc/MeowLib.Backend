using System.Text.Json.Serialization;

namespace MeowLib.Domain.Notification.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTypeEnum
{
    TeamInvite = 1,
    NewBookChapter = 2
}