using MeowLib.DAL;
using MeowLib.Domain.DbModels.NotificationEntity;
using MeowLib.Domain.DbModels.NotificationEntity.Payload;
using MeowLib.Domain.Dto.Notification;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Models;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITeamService _teamService;
    private readonly IJwtTokenService _jwtTokenService;
    
    public NotificationService(ApplicationDbContext dbContext, ITeamService teamService, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _teamService = teamService;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Метод отправляет уведомление произвольное уведомление пользователю.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="notificationType">Тип уведомления.</param>
    /// <param name="payload">Полезная нагрузка уведомления.</param>
    /// <returns>Результат отправки уведомления.</returns>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не найден.</exception>
    public async Task<Result> SendNotificationToUserAsync(int userId, NotificationTypeEnum notificationType, 
        BaseNotificationPayload payload)
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

    /// <summary>
    /// Метод отправляет пользователю уведомление с предложением вступить в комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результат отправки уведомления. Метод сохраняет все ошибки метода <see cref="SendNotificationToUserAsync"/></returns>
    /// <exception cref="TeamNotFoundException">Возникает в случае, если комманда не была найдена.</exception>
    public async Task<Result> SendInviteToTeamNotificationAsync(int teamId, int userId)
    {
        var foundedTeam = await _teamService.GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return Result.Fail(new TeamNotFoundException(teamId));
        }

        var inviteToken = _jwtTokenService.GenerateInviteToTeamStringToken(new InviteToTeamTokenModel
        {
            UserId = userId,
            TeamId = teamId,
            InviteExpiredAt = DateTime.UtcNow.AddDays(3)
        });

        var sendNotificationResult = await SendNotificationToUserAsync(userId, NotificationTypeEnum.TeamInvite, new TeamInviteNotificationPayload
        {
            InviteLink = inviteToken
        });

        if (sendNotificationResult.IsFailure)
        {
            return Result.Fail(sendNotificationResult.GetError());
        }
        
        return Result.Ok();
    }

    /// <summary>
    /// Метод возвращает список уведомлений пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список уведомлений пользователя.</returns>
    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        return await _dbContext.Notifications
            .Where(n => !n.IsWatched)
            .Where(n => n.User.Id == userId)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type,
                Payload = n.Payload,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync();
    }
}