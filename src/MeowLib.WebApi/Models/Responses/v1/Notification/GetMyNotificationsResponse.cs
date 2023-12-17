using MeowLib.Domain.Dto.Notification;

namespace MeowLib.WebApi.Models.Responses.v1.Notification;

public class GetMyNotificationsResponse
{
    public required IEnumerable<NotificationDto> Items { get; init; }
    public required int Count { get; init; }
}