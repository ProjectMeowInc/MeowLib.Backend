namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Базовый класс для исключений на уровне Services.
/// </summary>
public class ServiceLevelException : ApiException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceName">Название сервиса.</param>
    public ServiceLevelException(string serviceName) : base("Внутреняя ошибка сервера")
    {
        ServiceName = serviceName;
    }

    /// <summary>
    /// Название сервиса в котором произошло исключение.
    /// </summary>
    public string ServiceName { get; protected init; }
}