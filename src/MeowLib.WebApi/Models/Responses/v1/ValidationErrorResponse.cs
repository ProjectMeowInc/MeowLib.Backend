using MeowLib.Domain.Shared.Models;

namespace MeowLib.WebApi.Models.Responses.v1;

public class ValidationErrorResponse : BaseErrorResponse
{
    public ValidationErrorResponse(IEnumerable<ValidationErrorModel> validationErrors) : base("Ошибка валидации данных")
    {
        ValidationErrors = validationErrors;
    }

    public IEnumerable<ValidationErrorModel> ValidationErrors { get; set; }
}