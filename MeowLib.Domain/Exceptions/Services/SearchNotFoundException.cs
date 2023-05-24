namespace MeowLib.Domain.Exceptions.Services;

public class SearchNotFoundException : ServiceLevelException
{
    public SearchNotFoundException(string serviceName) : base(serviceName)
    {
    }
}