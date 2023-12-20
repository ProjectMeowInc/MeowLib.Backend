namespace MeowLib.Domain.Exceptions.Team;

public class UserAlreadyInTeamException : ApiException
{
    public UserAlreadyInTeamException(int userId, int teamId) :
        base($"Пользователь с Id = {userId} уже находится в комманде с Id = {teamId}") { }
}