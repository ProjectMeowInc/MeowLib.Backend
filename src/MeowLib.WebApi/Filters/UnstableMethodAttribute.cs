using Microsoft.AspNetCore.Mvc.Filters;

namespace MeowLib.WebApi.Filters;

public class UnstableMethodAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<UnstableMethodAttribute>>();
        logger.LogWarning("Использован нестабильный метод");
        context.HttpContext.Response.Headers.Append("Method-State", "Unstable");
        await next();
    }
}