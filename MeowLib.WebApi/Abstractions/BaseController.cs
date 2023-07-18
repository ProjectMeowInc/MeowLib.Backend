using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Responses;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

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
    protected new JsonResult Empty(int statusCode = 200)
    {
        return new JsonResult(null)
        {
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Метод возвращающий JSON-объект в виде <see cref="BaseErrorResponse"/>.
    /// </summary>
    /// <param name="errorMessage">Сообщение, котороые будет отправлены в модели ошибки.</param>
    /// <param name="statusCode">Статус код, который необходимо вернуть. Стандартное значение: 500.</param>
    /// <returns>Json-модель ответа.</returns>
    [NonAction]
    protected JsonResult Error(string errorMessage, int statusCode = 500)
    {
        return Json(new BaseErrorResponse(errorMessage), statusCode);
    }

    [NonAction]
    protected JsonResult UpdateAuthorizeResult()
    {
        return Json(new BaseErrorResponse("Произошла неизвестная ошибка. Попробуйте авторизоваться снова!"), 401);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки сервера в виде <see cref="BaseErrorResponse"/>.
    /// </summary>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult ServerError()
    {
        return Json(new BaseErrorResponse("Внутренняя ошибка сервера"), 500);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки 404 в виде <see cref="BaseErrorResponse"/>.
    /// </summary>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult NotFoundError()
    {
        return Json(new BaseErrorResponse("Сущность не найдена"), 404);
    }

    /// <summary>
    /// Метод возвращает стандартный формат ошибки 404 в виде <see cref="BaseErrorResponse"/>, с указанным сообщением.
    /// </summary>
    /// <param name="message">Сообщение для отправки.</param>
    /// <returns>Json-модель ответа.</returns>
    protected JsonResult NotFoundError(string message)
    {
        return Json(new BaseErrorResponse(message), 404);
    }
    
    
    [NonAction]
    protected async Task<UserDto> GetUserDataAsync()
    {
        if (HttpContext.Items.TryGetValue("UserData", out object? authData))
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
}