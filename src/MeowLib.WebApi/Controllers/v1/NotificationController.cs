using MeowLib.Domain.Notification.Exceptions;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Responses.v1.Notification;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер уведомлений.
/// </summary>
/// <param name="notificationService">Сервис уведомлений.</param>
/// <param name="logger">Логгер.</param>
[Route("api/v1/notifications")]
public class NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
    : BaseController
{
    /// <summary>
    /// Получение уведомление пользователя.
    /// </summary>
    [HttpGet("my")]
    [Authorization]
    [ProducesOkResponseType(typeof(GetMyNotificationsResponse))]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userData = await GetUserDataAsync();
        var notifications = await notificationService.GetUserNotificationsAsync(userData.Id);

        var notificationDtos = notifications.ToList();
        return Json(new GetMyNotificationsResponse
        {
            Items = notificationDtos.Select(n => new NotificationModel
            {
                Id = n.Id,
                Type = n.Type,
                Payload = n.Payload,
                CreatedAt = n.CreatedAt
            }),
            Count = notificationDtos.Count
        });
    }

    /// <summary>
    /// Отметить уведомление прочитанным.
    /// </summary>
    /// <param name="notificationId">Id уведомления</param>
    [HttpPost("my/watch/{notificationId}")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> WatchNotification([FromRoute] int notificationId)
    {
        var userRequestData = await GetUserDataAsync();
        var setNotificationAsWatchedResult =
            await notificationService.SetNotificationWatchedAsync(userRequestData.Id, notificationId);

        if (setNotificationAsWatchedResult.IsFailure)
        {
            var exception = setNotificationAsWatchedResult.GetError();
            if (exception is NotificationNotFoundException)
            {
                logger.LogWarning(
                    "Пользователь с Id = {userId} пытался удалить несуществующее уведомление с Id = {notificationId}",
                    userRequestData.Id, notificationId);
                return NotFoundError();
            }

            logger.LogError(
                "Неизвестная ошибка при попытке сделать уведомление пользователя просмотренным: {exception}",
                exception);
            return ServerError();
        }

        return Ok();
    }
}