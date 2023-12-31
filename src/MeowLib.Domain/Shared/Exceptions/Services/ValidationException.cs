using MeowLib.Domain.Models;

namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Класс для ошибок связанные с валидацией данных.
/// </summary>
public class ValidationException : ServiceLevelException
{
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
    public ValidationException(string serviceName, IEnumerable<ValidationErrorModel> validationErrors) :
        base(serviceName)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Список ошибок валидации.
    /// </summary>
    public IEnumerable<ValidationErrorModel> ValidationErrors { get; protected init; }
}