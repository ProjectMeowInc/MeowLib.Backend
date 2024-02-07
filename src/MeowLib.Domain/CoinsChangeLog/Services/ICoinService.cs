using MeowLib.Domain.CoinsChangeLog.Dto;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Exceptions;

namespace MeowLib.Domain.CoinsChangeLog.Services;

public interface ICoinService
{
    /// <summary>
    /// Метод изменяет количество монет пользователя от лица администратора.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="value">Значение для изменения.</param>
    /// <returns>Результат изменения.</returns>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователя не найден.</exception>
    Task<Result> ChangeCoinsByAdminAsync(int userId, decimal value);

    /// <summary>
    /// Метод получает список изменений монет пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Список изменений.</returns>
    Task<Result<List<CoinsChangeLogDto>>> GetUserCoinsChangeLogAsync(int userId);
}