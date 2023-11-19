using MeowLib.DAL;
using MeowLib.Domain.DbModels.CoinsChangeLogEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class CoinService(ApplicationDbContext dbContext) : ICoinService
{
    /// <summary>
    /// Метод изменяет количество монет пользователя от лица администратора.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="value">Значение для изменения.</param>
    /// <returns>Результат изменения.</returns>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователя не найден.</exception>
    public async Task<Result> ChangeCoinsByAdminAsync(int userId, decimal value)
    {
        var foundedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(userId));
        }

        // todo: add check result coins < 0?
        foundedUser.Coins += value;

        dbContext.Update(foundedUser);
        await dbContext.CoinsChangeLog.AddAsync(new CoinsChangeLogEntityModel
        {
            Type = CoinsChangeReasonTypeEnum.AdminAddition,
            Date = DateTime.UtcNow,
            User = foundedUser,
            Value = value
        });

        await dbContext.SaveChangesAsync();
        return Result.Ok();
    }
}