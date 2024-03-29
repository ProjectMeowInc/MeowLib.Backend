﻿using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeowLib.WebApi.Filters;

/// <summary>
/// Объявляет endpoint устаревшим.
/// </summary>
/// <param name="expiredDay">День устаревания.</param>
/// <param name="expiredMonth">Месяц устаревания.</param>
/// <param name="expiredYear">Год устаревания.</param>
public class DeprecatedMethodAttribute(int expiredDay, int expiredMonth, int expiredYear)
    : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers.Append("Deprecated-Date", $"{expiredDay}.{expiredMonth}.{expiredYear}");
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DeprecatedMethodAttribute>>();

        logger.LogWarning("Попытка использовать устаревший метод: {method} {path}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path);

        if (IsEndpointDeprecated())
        {
            context.Result = new JsonResult(new BaseErrorResponse(
                $"Запрашиваемый метод был признан устаревшим начиная с {expiredDay}.{expiredMonth}.{expiredYear}"));
            return;
        }

        await next();
    }

    private bool IsEndpointDeprecated()
    {
        var currentData = DateTime.UtcNow;
        var deprecatedData = new DateTime(expiredYear, expiredMonth, expiredDay);
        return currentData >= deprecatedData;
    }
}