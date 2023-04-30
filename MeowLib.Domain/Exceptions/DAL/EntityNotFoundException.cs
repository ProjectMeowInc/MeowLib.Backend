namespace MeowLib.Domain.Exceptions.DAL;

/// <summary>
/// Класс исключений для случаев когда сущность не была найдена.
/// </summary>
public class EntityNotFoundException : DalLevelException
{
    /// <summary>
    /// Параметр по каторому был произведён поиск.
    /// </summary>
    public string RequestParam { get; protected set; } = null!;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="entityName">Название сущности.</param>
    /// <param name="requestParam">Параметр по каторому был произведён поиск.</param>
    public EntityNotFoundException(string entityName, string requestParam)
    {
        EntityName = entityName;
        RequestParam = requestParam;
    }
}