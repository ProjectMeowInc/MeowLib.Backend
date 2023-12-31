using MeowLib.Domain.Shared;

namespace MeowLib.Domain.Team.Exceptions;

public class UserAlreadyInTeamException : ApiException
{
    public UserAlreadyInTeamException(int userId, int teamId) :
        base($"Пользователь с Id = {userId} уже находится в комманде с Id = {teamId}") { }
}