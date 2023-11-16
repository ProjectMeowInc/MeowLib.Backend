namespace MeowLib.WebApi.Models.Responses;

public class BaseErrorResponse
{
    public string ErrorMessage { get; set; }

    public BaseErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}