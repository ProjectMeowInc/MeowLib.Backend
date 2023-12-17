using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MeowLib.WebApi.Filters;

public class DeprecatedMethodAttribute(int expiredDay, int expiredMonth, int expiredYear) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        context.HttpContext.Response.Headers.Append("Deprecated-Date", $"{expiredDay}.{expiredMonth}.{expiredYear}");
        
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
        if (currentData.Year > expiredYear)
        {
            return true;
        }

        if (currentData.Month > expiredMonth)
        {
            return true;
        }

        if (currentData.Day > expiredDay)
        {
            return true;
        }

        return false;
    }
}