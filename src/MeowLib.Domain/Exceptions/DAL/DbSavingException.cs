using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Exceptions.DAL;

/// <summary>
/// Класс исключений для ситуаций связанных с ошибкой сохранения данных в БД.
/// </summary>
public class DbSavingException : DalLevelException
{
    public DbSavingException(string entityName, DbSavingTypesEnum action) : base(entityName)
    {
        Action = action;
    }

    /// <summary>
    /// Тип действия при котором произошла ошибка.
    /// </summary>
    public DbSavingTypesEnum Action { get; protected set; }
}