namespace MeowLib.Domain.Notification.Exceptions;

public class NotificationNotFoundException(int notificationId) : Exception(
    $"Уведомление с Id = {notificationId} не найдено");