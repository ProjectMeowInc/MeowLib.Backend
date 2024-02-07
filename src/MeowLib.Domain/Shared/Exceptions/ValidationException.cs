using MeowLib.Domain.Shared.Models;

namespace MeowLib.Domain.Shared.Exceptions;

/// <summary>
/// Класс для ошибок связанные с валидацией данных.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="validationErrors">Список ошибок валидации.</param>
    public ValidationException(IEnumerable<ValidationErrorModel> validationErrors)
    {
        ValidationErrors = validationErrors;
    }

    /// <summary>
    /// Список ошибок валидации.
    /// </summary>
    public IEnumerable<ValidationErrorModel> ValidationErrors { get; }
}