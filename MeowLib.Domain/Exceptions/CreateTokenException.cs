namespace MeowLib.Domain.Exceptions;

public class CreateTokenException : ApiException
{
    public CreateTokenException(string errorMessage) : base(errorMessage)
    {
    }
}