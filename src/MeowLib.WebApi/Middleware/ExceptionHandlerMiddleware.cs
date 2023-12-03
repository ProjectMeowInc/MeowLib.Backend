using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.WebApi.Models.Responses;

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
        var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandlerMiddleware>>();

        try
        {
            logger.LogInformation("[{@DateTime}] Начало обработки запроса", DateTime.UtcNow);
            await _next.Invoke(context);
            logger.LogInformation("[{@DateTime}] Запрос успешно обработан", DateTime.UtcNow);
        }
        catch (ValidationException validationException)
        {
            logger.LogError("[{@DateTime}] Произошла ошибка валидации: {@ValidationsError}",
                DateTime.UtcNow, validationException.ValidationErrors);

            if (context.Response.HasStarted)
            {
                logger.LogCritical("[{@DateTime}] Ответ уже отправлен пользователю", DateTime.UtcNow);
                return;
            }

            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new ValidationErrorResponse(validationException.ValidationErrors));
            logger.LogInformation("[{@DateTime}] Ответ отправлен пользователю", DateTime.UtcNow);
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

            logger.LogError("[{DateTime}] Произошло неожиданное исключение: {@ExceptionMessage}, {@Exception}",
                DateTime.UtcNow, exception.Message, exception);

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                ErrorMessage = "Ошибка сервера"
            });
        }
    }
}