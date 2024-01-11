using MeowLib.Domain.CoinsChangeLog.Services;
using MeowLib.Domain.User.Enums;
using MeowLib.Domain.User.Exceptions;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Coin;
using MeowLib.WebApi.Models.Responses.v1.Coin;
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

    [HttpGet("my")]
    [Authorization]
    [ProducesOkResponseType(typeof(GetCoinChangeLogsResponse))]
    public async Task<IActionResult> GetMyChangeCoinsLog()
    {
        var userData = await GetUserDataAsync();
        var getLogsResult = await coinService.GetUserCoinsChangeLogAsync(userData.Id);
        if (getLogsResult.IsFailure)
        {
            var exception = getLogsResult.GetError();
            logger.LogError("Ошибка получения логов изменения монет пользователя: {exception}", exception);
            return ServerError();
        }

        var models = getLogsResult.GetResult().Select(log => new CoinChangeLogModel
        {
            Id = log.Id,
            Value = log.Value,
            Reason = log.Type,
            Date = log.Date
        });

        return Ok(new GetCoinChangeLogsResponse
        {
            Items = models
        });
    }

    [HttpGet("admin-get/{userId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    public async Task<IActionResult> GetUserChangeCoinLog([FromRoute] int userId)
    {
        var getLogsResult = await coinService.GetUserCoinsChangeLogAsync(userId);
        if (getLogsResult.IsFailure)
        {
            var exception = getLogsResult.GetError();
            logger.LogError("Ошибка получения логов изменения монет пользователя: {exception}", exception);
            return ServerError();
        }

        var models = getLogsResult.GetResult().Select(log => new CoinChangeLogModel
        {
            Id = log.Id,
            Value = log.Value,
            Reason = log.Type,
            Date = log.Date
        });

        return Ok(new GetCoinChangeLogsResponse
        {
            Items = models
        });
    }
}