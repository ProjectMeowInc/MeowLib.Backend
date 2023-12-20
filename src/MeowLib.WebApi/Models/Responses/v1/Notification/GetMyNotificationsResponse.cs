namespace MeowLib.WebApi.Models.Responses.v1.Notification;

public class GetMyNotificationsResponse
{
    public required IEnumerable<NotificationModel> Items { get; init; }
    public required int Count { get; init; }
}