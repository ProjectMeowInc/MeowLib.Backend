namespace MeowLib.Domain.Exceptions;

/// <summary>
/// Класс исключений, для ошибок связанных с внутренней реализацией (н.р. ошибка сохранения в БД)
/// </summary>
public class InnerException : ApiException
{
    public InnerException(string errorMessage) : base(errorMessage)
    {
    }
}