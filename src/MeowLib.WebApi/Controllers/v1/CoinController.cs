using MeowLib.Domain.CoinsChangeLog.Services;
using MeowLib.Domain.User.Enums;
using MeowLib.Domain.User.Exceptions;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Coin;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер монет.
/// </summary>
/// <param name="coinService">Сервис монет.</param>
/// <param name="logger">Логгер.</param>
[Route("api/v1/coins")]
public class CoinController(ICoinService coinService, ILogger<CoinController> logger) : BaseController
{
    /// <summary>
    /// Обновление монет пользователя администратором.
    /// </summary>
    /// <remarks>Стоит учитывать, что манеты изменяются на указанную сумма, а не устаналиваются в неё.</remarks>
    /// <param name="payload"></param>
    [HttpPost("admin-change")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> UpdateCoinsByAdmin([FromBody] ChangeCoinsByAdminRequest payload)
    {
        var changeCoinsResult = await coinService.ChangeCoinsByAdminAsync(payload.UserId, payload.UpdateCoins);
        if (changeCoinsResult.IsFailure)
        {
            var exception = changeCoinsResult.GetError();
            if (exception is UserNotFoundException)
            {
                return Error("Запрашиваемый пользователь не найден.");
            }

            logger.LogError("Неизвестная ошибка при обновлении монет пользователя администратором: {exception}",
                exception);
            return ServerError();
        }

        return Ok();
    }
}