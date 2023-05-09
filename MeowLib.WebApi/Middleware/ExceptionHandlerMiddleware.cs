using MeowLib.Domain.Exceptions;

namespace MeowLib.WebApi.Middleware;

/// <summary>
/// Хендлер ошибок.
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (ApiException apiException)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(apiException);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            
            Console.WriteLine("Неожиданное исключение!");
            Console.WriteLine(exception.Message);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                ErrorMessage = "Ошибка сервера"
            });
        }
    }
}