using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.Coin;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/coins")]
public class CoinController(ICoinService coinService, ILogger<CoinController> logger) : BaseController
{
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