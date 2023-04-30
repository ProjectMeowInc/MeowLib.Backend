using MeowLib.Domain.Models;

namespace MeowLib.Domain.Exceptions.Services;

public class ValidationException : ApiException
{
    public IEnumerable<ValidationErrorModel> ValidationErrors { get; protected set; }
    
    public ValidationException(IEnumerable<ValidationErrorModel> validationErrors)
    {
        ErrorMessage = "Некорректные входные данные";
        ValidationErrors = validationErrors;
    }
}