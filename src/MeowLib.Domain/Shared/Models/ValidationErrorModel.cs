namespace MeowLib.Domain.Shared.Models;

/// <summary>
/// Модель ошибки валидации.
/// </summary>
public class ValidationErrorModel
{
    /// <summary>
    /// Название свойства.
    /// </summary>
    public required string PropertyName { get; init; }

    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public required string Message { get; init; }
}