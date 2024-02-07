using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Team.Exceptions;

public class TeamOwnerNotFoundException : ApiException
{
    public TeamOwnerNotFoundException(int ownerId) : base(
        $"Пользователь с Id = {ownerId} не найден. Комманда не создана") { }
}