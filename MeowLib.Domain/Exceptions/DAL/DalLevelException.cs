namespace MeowLib.Domain.Exceptions.DAL;

/// <summary>
/// Базовый класс для исключений на уровне DAL.
/// </summary>
public abstract class DalLevelException : ApiException
{
    /// <summary>
    /// Название сущности на уровне DAL.
    /// </summary>
    public string EntityName { get; protected set; } = null!;
}