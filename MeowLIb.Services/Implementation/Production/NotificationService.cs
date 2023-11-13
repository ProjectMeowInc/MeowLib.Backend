using MeowLib.DAL;
using MeowLib.Domain.Dto.Notification;
using MeowLib.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class NotificationService
{
    private readonly ApplicationDbContext _dbContext;
    
    public NotificationService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IEnumerable<NotificationDto>>> GetUserNotifications(int userId)
    {
        return await _dbContext.Notifications
            .Where(n => n.User.Id == userId && n.IsWatched == false)
            .Select(n => new NotificationDto
            {
                Id = n.Id,
                CreatedAt = n.CreatedAt,
                Payload = n.Payload,
                Type = n.Type
            })
            .ToListAsync();
    }
}