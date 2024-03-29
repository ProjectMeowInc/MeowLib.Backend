using System.Diagnostics;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Team.Dto;
using MeowLib.Domain.User.Dto;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MeowLib.WebApi.Abstractions;

[ApiController]
[ProducesServerErrorResponseType]
public class BaseController : ControllerBase
{
    [NonAction]
    protected JsonResult Json(object content, int statusCode = 200)
    {
        return new JsonResult(content)
        {
            StatusCode = statusCode
        };
    }

    [NonAction]
    protected IActionResult EmptyResult(int statusCode = 200)
    {
        return StatusCode(200);
    }

    /// <summary>
    /// Метод возвращающий JSON-объект в виде <see cref="BaseErrorResponse" />.
    /// </summary>
    /// <param name="errorMessage">Сообщение, котороые будет отправлены в модели ошибки.</param>
    /// <param name="statusCode">Статус код, который необходимо вернуть. Стандартное значение: 500.</param>
    /// <returns>Json-модель ответа.</returns>
    [NonAction]
    protected JsonResult Error(string errorMessage, int statusCode = 500)
    {
        return Json(new BaseErrorResponse(errorMessage), statusCode);
    }

    /// <summary>
    /// Метод возвращает ответ с кодом 401 с просьбой обновить авторизацию.
    /// </summary>
    /// <returns>Json-модель ответа.</returns>
    [NonAction]
    protected JsonResult UpdateAuthorizeError()
    {
        return Json(new BaseErrorResponse("Произошла неизвестная ошибка. Попробуйте авторизоваться снова"), 401);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки сервера в виде <see cref="BaseErrorResponse" />.
    /// </summary>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult ServerError()
    {
        return Json(new BaseErrorResponse("Внутренняя ошибка сервера"), 500);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки 404 в виде <see cref="BaseErrorResponse" />.
    /// </summary>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult NotFoundError()
    {
        return Json(new BaseErrorResponse("Сущность не найдена"), 404);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки 404 в виде <see cref="BaseErrorResponse" />, с указанным сообщением.
    /// </summary>
    /// <param name="message">Сообщение для отправки.</param>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult NotFoundError(string message)
    {
        return Json(new BaseErrorResponse(message), 404);
    }

    protected JsonResult ValidationError(IEnumerable<ValidationErrorModel> errors)
    {
        return Json(new ValidationErrorResponse(errors), 400);
    }

    [NonAction]
    protected async Task<UserDto> GetUserDataAsync()
    {
        if (HttpContext.Items.TryGetValue("UserData", out var authData))
        {
            if (authData is null)
            {
                Response.StatusCode = 500;
                await Response.WriteAsJsonAsync(new
                {
                    ErrorMessage = "Ошибка авторизации"
                });
                throw new ApiException("Не найдены данные для авторизации");
            }

            if (authData is not UserDto parsedData)
            {
                Response.StatusCode = 500;
                await Response.WriteAsJsonAsync(new
                {
                    ErrorMessage = "Ошибка авторизации"
                });
                throw new ApiException("Данные для авторизации находятся не в том формате");
            }

            return parsedData;
        }

        Response.StatusCode = 500;
        await Response.WriteAsJsonAsync(new
        {
            ErrorMessage = "Ошибка авторизации"
        });
        throw new ApiException("Не найдены данные для авторизации");
    }

    [NonAction]
    protected List<TeamDto> GetUserTeams()
    {
        if (HttpContext.Items.TryGetValue("UserTeams", out var data))
        {
            if (data is List<TeamDto> userTeams)
            {
                return userTeams;
            }
        }

        throw new UnreachableException("Список комманд пользователя не может быть null");
    }
}