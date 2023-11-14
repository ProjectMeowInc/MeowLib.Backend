using MeowLib.DAL;
using MeowLib.Domain.DbModels.NotificationEntity;
using MeowLib.Domain.Dto.Notification;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Notification;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Models;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;

    public NotificationService(ApplicationDbContext dbContext, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result> SendNotificationToUserAsync(int userId, NotificationTypeEnum notificationType,
        string payload)
    {
        var foundedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        await _dbContext.Notifications.AddAsync(new NotificationEntityModel
        {
            Type = notificationType,
            Payload = payload,
            CreatedAt = DateTime.UtcNow,
            IsWatched = false,
            User = foundedUser
        });
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> SendInviteToTeamNotificationAsync(int teamId, int userId)
    {

        var inviteToken = _jwtTokenService.GenerateInviteToTeamStringToken(new InviteToTeamTokenModel
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
        return await _dbContext.Notifications
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
        var foundedNotification = await _dbContext.Notifications
            .Where(n => !n.IsWatched)
            .Where(n => n.User.Id == userId)
            .FirstOrDefaultAsync(n => n.Id == notificationId);

        if (foundedNotification is null)
        {
            return Result.Fail(new NotificationNotFoundException(notificationId));
        }

        foundedNotification.IsWatched = true;

        _dbContext.Update(foundedNotification);
        await _dbContext.SaveChangesAsync();

        return Result.Ok();
    }    
}