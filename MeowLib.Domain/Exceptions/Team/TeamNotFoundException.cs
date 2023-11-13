namespace MeowLib.Domain.Exceptions.Team;

public class TeamNotFoundException : ApiException
{
    public TeamNotFoundException(int teamId) : base($"Команда с Id = {teamId} не найдена") { }
}