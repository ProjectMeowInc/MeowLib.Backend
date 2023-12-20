namespace MeowLib.WebApi.Models.Responses.v1;

public class BaseErrorResponse
{
    public BaseErrorResponse(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public string ErrorMessage { get; set; }
}