using MeowLib.Domain.Models;

namespace MeowLib.WebApi.Models.Responses;

public class ValidationErrorResponse : BaseErrorResponse
{
    public ValidationErrorResponse(IEnumerable<ValidationErrorModel> validationErrors) : base("Ошибка валидации данных")
    {
        ValidationErrors = validationErrors;
    }

    public IEnumerable<ValidationErrorModel> ValidationErrors { get; set; }
}