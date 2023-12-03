using MeowLib.Domain.Exceptions.Notification;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Responses.Notification;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/notifications")]
public class NotificationController : BaseController
{
    private readonly ILogger<NotificationController> _logger;
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("my")]
    [Authorization]
    [ProducesOkResponseType(typeof(GetMyNotificationsResponse))]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userData = await GetUserDataAsync();
        var notifications = await _notificationService.GetUserNotificationsAsync(userData.Id);

        var notificationDtos = notifications.ToList();
        return Json(new GetMyNotificationsResponse
        {
            Items = notificationDtos,
            Count = notificationDtos.Count
        });
    }

    [HttpPost("my/watch/{notificationId}")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> WatchNotification([FromRoute] int notificationId)
    {
        var userRequestData = await GetUserDataAsync();
        var setNotificationAsWatchedResult =
            await _notificationService.SetNotificationWatchedAsync(userRequestData.Id, notificationId);

        if (setNotificationAsWatchedResult.IsFailure)
        {
            var exception = setNotificationAsWatchedResult.GetError();
            if (exception is NotificationNotFoundException)
            {
                _logger.LogWarning(
                    "Пользователь с Id = {userId} пытался удалить несуществующее уведомление с Id = {notificationId}",
                    userRequestData.Id, notificationId);
                return NotFoundError();
            }

            _logger.LogError(
                "Неизвестная ошибка при попытке сделать уведомление пользователя просмотренным: {exception}",
                exception);
            return ServerError();
        }

        return Ok();
    }
}