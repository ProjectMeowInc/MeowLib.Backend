using System.Text;
using System.Text.Json;
using MeowLib.DAL;
using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Notification.Dto;
using MeowLib.Domain.Notification.Entity;
using MeowLib.Domain.Notification.Enums;
using MeowLib.Domain.Notification.Exceptions;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Exceptions;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class NotificationService(ApplicationDbContext dbContext, INotificationTokenService notificationTokenService)
    : INotificationService
{
    public async Task<Result> SendNotificationToUserAsync(int userId, NotificationTypeEnum notificationType,
        string payload)
    {
        var foundedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        await dbContext.Notifications.AddAsync(new NotificationEntityModel
        {
            Type = notificationType,
            Payload = payload,
            CreatedAt = DateTime.UtcNow,
            IsWatched = false,
            User = foundedUser
        });
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> SendInviteToTeamNotificationAsync(int teamId, int userId)
    {
        var inviteToken = notificationTokenService.GenerateInviteToTeamStringToken(new InviteToTeamTokenModel
        {
            UserId = userId,
            TeamId = teamId,
            InviteExpiredAt = DateTime.UtcNow.AddDays(3)
        });

        var sendNotificationResult =
            await SendNotificationToUserAsync(userId, NotificationTypeEnum.TeamInvite, inviteToken);

        if (sendNotificationResult.IsFailure)
        {
            return Result.Fail(sendNotificationResult.GetError());
        }

        return Result.Ok();
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        return await dbContext.Notifications
            .Where(n => !n.IsWatched)
            .Where(n => n.User.Id == userId)
            .AsNoTracking()
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Payload = n.Payload,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<Result> SetNotificationWatchedAsync(int userId, int notificationId)
    {
        var foundedNotification = await dbContext.Notifications
            .Where(n => !n.IsWatched)
            .Where(n => n.User.Id == userId)
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (foundedNotification is null)
        {
            return Result.Fail(new NotificationNotFoundException(notificationId));
        }

        foundedNotification.IsWatched = true;

        dbContext.Update(foundedNotification);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    /// <summary>
    /// Метод отправляет уведомления пользователям о выходе новой главы.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="chapterName">Название главы.</param>
    /// <returns>Результат отправки уведомлений.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    public async Task<Result> SendNotificationToBookSubscribersAsync(int bookId, string chapterName)
    {
        var foundedBook = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
        if (foundedBook is null)
        {
            return Result.Fail(new BookNotFoundException(bookId));
        }

        var users = dbContext.UsersFavorite
            .Where(uf => uf.Book.Id == bookId)
            .Select(uf => uf.User);

        var serializedPayload = JsonSerializer.Serialize(new
        {
            bookName = foundedBook.Name, chapterName
        });

        var payloadBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedPayload));

        var notifications = new List<NotificationEntityModel>();
        foreach (var userEntityModel in users)
        {
            notifications.Add(new NotificationEntityModel
            {
                Type = NotificationTypeEnum.NewBookChapter,
                Payload = payloadBase64,
                IsWatched = false,
                CreatedAt = DateTime.UtcNow,
                User = userEntityModel
            });
        }

        await dbContext.Notifications.AddRangeAsync(notifications);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }
}