namespace MeowLib.Domain.Exceptions.Team;

public class ChangeOwnerRoleException : ApiException
{
    public ChangeOwnerRoleException() : base("Попытка изменить роль владельца команды") { }
}