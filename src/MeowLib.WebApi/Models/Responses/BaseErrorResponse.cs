namespace MeowLib.WebApi.Models.Responses;

public class BaseErrorResponse
{
    public BaseErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; set; }
}