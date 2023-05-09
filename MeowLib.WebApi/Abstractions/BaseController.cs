using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Abstractions;

[ApiController]
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
    protected JsonResult Error(string errorMessage, int statusCode = 200)
    {
        return Json(new BaseErrorResponse(errorMessage), statusCode);
    }
    
    [NonAction]
    protected async Task<UserDto> GetUserData()
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