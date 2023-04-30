namespace MeowLib.Domain.Responses;

public class BaseErrorResponse
{
    public string ErrorMessage { get; set; }

    public BaseErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}