using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Team.Exceptions;

public class TeamNotFoundException : ApiException
{
    public TeamNotFoundException(int teamId) : base($"Команда с Id = {teamId} не найдена") { }
}