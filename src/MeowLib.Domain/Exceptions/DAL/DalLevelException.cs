namespace MeowLib.Domain.Exceptions.DAL;

/// <summary>
/// Базовый класс для исключений на уровне DAL.
/// </summary>
public abstract class DalLevelException : ApiException
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="entityName">Название сущности.</param>
    public DalLevelException(string entityName) : base($"Сущность {entityName} не найдена")
    {
        EntityName = entityName;
    }

    /// <summary>
    /// Название сущности на уровне DAL.
    /// </summary>
    public string EntityName { get; protected set; }
}