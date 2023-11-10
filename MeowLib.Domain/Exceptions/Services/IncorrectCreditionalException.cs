namespace MeowLib.Domain.Exceptions.Services;

/// <summary>
/// Класс описывающий исключения для некорректных авторизационные данных.
/// </summary>
public class IncorrectCreditionalException : ApiException
{
    public IncorrectCreditionalException(string errorMessage) : base(errorMessage)
    {
    }
}