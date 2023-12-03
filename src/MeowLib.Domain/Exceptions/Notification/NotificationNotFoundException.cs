namespace MeowLib.Domain.Exceptions.Notification;

public class NotificationNotFoundException(int notificationId) : Exception(
    $"Уведомление с Id = {notificationId} не найдено");