namespace MeowLib.Domain.Exceptions;

/// <summary>
/// Базовый класс для всех исключений внутри API
/// </summary>
public class ApiException : Exception
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="errorMessage">Сообщение.</param>
    public ApiException(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; protected init; }
}