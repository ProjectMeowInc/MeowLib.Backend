namespace MeowLib.Domain.Exceptions;

/// <summary>
/// Класс исключений, связанных с созданием токена.
/// </summary>
public class CreateTokenException : ApiException
{
    public CreateTokenException(string errorMessage) : base(errorMessage) { }
}