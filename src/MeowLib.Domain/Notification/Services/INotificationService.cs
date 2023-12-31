using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Notification.Dto;
using MeowLib.Domain.Notification.Enums;
using MeowLib.Domain.Notification.Exceptions;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Exceptions;

namespace MeowLib.Domain.Notification.Services;

public interface INotificationService
{
    /// <summary>
    /// Метод отправляет уведомление произвольное уведомление пользователю.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="notificationType">Тип уведомления.</param>
    /// <param name="payload">Полезная нагрузка уведомления.</param>
    /// <returns>Результат отправки уведомления.</returns>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не найден.</exception>
    Task<Result> SendNotificationToUserAsync(int userId, NotificationTypeEnum notificationType,
        string payload);

    /// <summary>
    /// Метод отправляет пользователю уведомление с предложением вступить в комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>
    /// Результат отправки уведомления. Метод сохраняет все ошибки метода <see cref="SendNotificationToUserAsync" />
    /// </returns>
    Task<Result> SendInviteToTeamNotificationAsync(int teamId, int userId);

    /// <summary>
    /// Метод возвращает список уведомлений пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список уведомлений пользователя.</returns>
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);

    /// <summary>
    /// Метод делает уведомление пользователя просмотренным.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="notificationId">Id уведомления.</param>
    /// <returns>Результат просмотра уведомления</returns>
    /// <exception cref="NotificationNotFoundException">Возникает в случае, если уведомление не было найдено.</exception>
    Task<Result> SetNotificationWatchedAsync(int userId, int notificationId);

    /// <summary>
    /// Метод отправляет уведомления пользователям о выходе новой главы.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="chapterName">Название главы.</param>
    /// <returns>Результат отправки уведомлений.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    Task<Result> SendNotificationToBookSubscribersAsync(int bookId, string chapterName);
}