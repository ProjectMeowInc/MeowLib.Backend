using MeowLib.Domain.DbModels.NotificationEntity.Payload;
using MeowLib.Domain.Dto.Notification;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;

namespace MeowLib.Services.Interface;

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
        BaseNotificationPayload payload);
    
    /// <summary>
    /// Метод отправляет пользователю уведомление с предложением вступить в комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результат отправки уведомления. Метод сохраняет все ошибки метода <see cref="SendNotificationToUserAsync"/></returns>
    /// <exception cref="TeamNotFoundException">Возникает в случае, если комманда не была найдена.</exception>
    Task<Result> SendInviteToTeamNotificationAsync(int teamId, int userId);
    
    /// <summary>
    /// Метод возвращает список уведомлений пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список уведомлений пользователя.</returns>
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
}