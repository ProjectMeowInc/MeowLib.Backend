using MeowLib.Domain.Shared;

namespace MeowLib.Domain.User.Exceptions;

public class UserNotFoundException : ApiException
{
    public UserNotFoundException(int userId) : base($"Пользователь с Id = {userId} не найден") { }
}