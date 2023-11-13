namespace MeowLib.Domain.DbModels.NotificationEntity.Payload;

public class TeamInviteNotificationPayload : BaseNotificationPayload
{
    public required string InviteLink { get; init; }
}