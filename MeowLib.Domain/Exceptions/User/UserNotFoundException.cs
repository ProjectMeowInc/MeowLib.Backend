namespace MeowLib.Domain.Exceptions.User;

public class UserNotFoundException : ApiException
{
    public UserNotFoundException(int userId) : base($"Пользователь с Id = {userId} не найден")
    {
    }
}