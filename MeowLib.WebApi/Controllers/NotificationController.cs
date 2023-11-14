using MeowLib.Domain.Responses.Notification;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/notifications")]
public class NotificationController : BaseController
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("my"), Authorization]
    [ProducesOkResponseType(typeof(GetMyNotificationsResponse))]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userData = await GetUserDataAsync();
        var notifications = await _notificationService.GetUserNotificationsAsync(userData.Id);

        var notificationDtos = notifications.ToList();
        return Json(new GetMyNotificationsResponse
        {
            Items = notificationDtos,
            Count = notificationDtos.Count,
        });
    }
}