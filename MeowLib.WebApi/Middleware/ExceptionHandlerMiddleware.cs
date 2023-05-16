using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Responses;

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
        catch (ValidationException validationException)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ValidationErrorResponse(validationException.ValidationErrors));
        }
        catch (DbSavingException)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new BaseErrorResponse("Внутреняя ошибка сервера"));
        }
        catch (DalLevelException)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new BaseErrorResponse("Внутреняя ошибка сервера"));
        }
        catch (ApiException apiException)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new BaseErrorResponse(apiException.ErrorMessage));
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                return;
            }
            
            Console.WriteLine("Неожиданное исключение!");
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.StackTrace);
            
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                ErrorMessage = "Ошибка сервера"
            });
        }
    }
}