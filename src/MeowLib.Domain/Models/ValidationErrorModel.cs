namespace MeowLib.Domain.Models;

/// <summary>
/// Модель ошибки валидации.
/// </summary>
public class ValidationErrorModel
{
    /// <summary>
    /// Название свойства.
    /// </summary>
    public required string PropertyName { get; set; }

    /// <summary>
    /// Текст ошибки.
    /// </summary>
    public required string Message { get; set; }
}