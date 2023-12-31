using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Team.Exceptions;

public class ChangeOwnerRoleException : ApiException
{
    public ChangeOwnerRoleException() : base("Попытка изменить роль владельца команды") { }
}