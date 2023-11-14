using System.Text.Json.Serialization;

namespace MeowLib.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationTypeEnum
{
    TeamInvite = 1,
}