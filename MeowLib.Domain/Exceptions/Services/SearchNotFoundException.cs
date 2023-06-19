namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Класс исключения для ситуаций, когда запрос пользователя на поиск не выдал никаких результатов.
/// </summary>
public class SearchNotFoundException : ServiceLevelException
{
    public SearchNotFoundException(string serviceName) : base(serviceName)
    {
    }
}