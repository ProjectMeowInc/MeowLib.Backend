using MeowLib.Domain.Dto.Notification;

namespace MeowLib.WebApi.Models.Responses.Notification;

public class GetMyNotificationsResponse
{
    public required IEnumerable<NotificationDto> Items { get; init; }
    public required int Count { get; init; }
}