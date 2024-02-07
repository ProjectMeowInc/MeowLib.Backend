namespace MeowLib.Domain.Shared;

/// <summary>
/// Класс исключений, для ошибок связанных с внутренней реализацией (н.р. ошибка сохранения в БД)
/// </summary>
public class InnerException(string errorMessage) : Exception(errorMessage);