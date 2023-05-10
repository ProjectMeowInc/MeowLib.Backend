using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Exceptions.DAL;

public class DbSavingException : DalLevelException
{
    public DbSavingTypesEnum Action { get; protected set; }
    
    public DbSavingException(string entityName, DbSavingTypesEnum action) : base(entityName)
    {
        Action = action;
    }
}