namespace MeowLib.Domain.Exceptions.Services;

public class IncorrectCreditionalException : ApiException
{
    public IncorrectCreditionalException(string errorMessage) : base(errorMessage)
    {
        
    }
}