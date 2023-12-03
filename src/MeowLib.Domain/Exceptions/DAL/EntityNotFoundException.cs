namespace MeowLib.Domain.Exceptions.DAL;

/// <summary>
/// Класс исключений для случаев когда сущность не была найдена.
/// </summary>
public class EntityNotFoundException : DalLevelException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="entityName">Название сущности.</param>
    /// <param name="requestParam">Параметр по каторому был произведён поиск.</param>
    public EntityNotFoundException(string entityName, string requestParam)
        : base($"Сущность {entityName} по параметру {requestParam} не найдена")
    {
        EntityName = entityName;
        RequestParam = requestParam;
    }

    /// <summary>
    /// Параметр по каторому был произведён поиск.
    /// </summary>
    public string RequestParam { get; protected set; }
}