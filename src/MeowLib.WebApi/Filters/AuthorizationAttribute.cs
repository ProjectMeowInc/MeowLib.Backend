using MeowLib.Domain.User.Enums;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace MeowLib.WebApi.Filters;

public class AuthorizationAttribute : ProducesResponseTypeAttribute, IAsyncAuthorizationFilter
{
    public AuthorizationAttribute() : base(typeof(BaseErrorResponse), 401) { }

    public AuthorizationAttribute(Type type, int statusCode, string contentType, params string[] additionalContentTypes)
        : base(type, statusCode, contentType, additionalContentTypes) { }

    public UserRolesEnum[] RequiredRoles { get; set; } = [];

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authToken = context.HttpContext.Request.Headers["Authorization"];
        if (authToken.Equals(StringValues.Empty))
        {
            context.Result = new JsonResult(new BaseErrorResponse("Токен доступа не найден"))
            {
                StatusCode = 401
            };
            return;
        }

        var jwtTokenService = context.HttpContext.RequestServices.GetService<IUserTokenService>();
        if (jwtTokenService is null)
        {
            context.Result = new JsonResult(new BaseErrorResponse("Ошибка сервера"))
            {
                StatusCode = 500
            };
            return;
        }

        var parsedTokenData =
            await jwtTokenService.ParseAccessTokenAsync(authToken.ToString().Replace("Bearer ", string.Empty));
        if (parsedTokenData is null)
        {
            context.Result = new JsonResult(new BaseErrorResponse("Ошибка валидации токена"))
            {
                StatusCode = 401
            };
            return;
        }

        if (RequiredRoles.Any())
        {
            if (!RequiredRoles.Contains(parsedTokenData.Role))
            {
                context.Result = new JsonResult(new BaseErrorResponse("Нет доступа"))
                {
                    StatusCode = 403
                };
                return;
            }
        }

        context.HttpContext.Items.Add("UserData", parsedTokenData);
    }
}