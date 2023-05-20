using MeowLib.Domain.Interfaces;
using MeowLib.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Класс для ошибок связанные с валидацией данных.
/// </summary>
public class ValidationException : ServiceLevelException, IHasResponseForm
{
    /// <summary>
    /// Список ошибок валидации.
    /// </summary>
    public IEnumerable<ValidationErrorModel> ValidationErrors { get; protected set; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="validationErrors">Список ошибок валидации.</param>
    public ValidationException(IEnumerable<ValidationErrorModel> validationErrors) : base("N/A")
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceName">Название сервиса.</param>
    /// <param name="validationErrors">Список ошибок валидации.</param>
    public ValidationException(string serviceName, IEnumerable<ValidationErrorModel> validationErrors) : base(serviceName)
    {
        ValidationErrors = validationErrors;
    }

    public JsonResult ToResponse()
    {
        return new JsonResult(ValidationErrors)
        {
            StatusCode = 403
        };
    }
}