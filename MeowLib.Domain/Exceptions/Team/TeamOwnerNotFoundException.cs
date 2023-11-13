namespace MeowLib.Domain.Exceptions.Team;

public class TeamOwnerNotFoundException : ApiException
{
    public TeamOwnerNotFoundException(int ownerId) : base($"Пользователь с Id = {ownerId} не найден. Комманда не создана") { }
}