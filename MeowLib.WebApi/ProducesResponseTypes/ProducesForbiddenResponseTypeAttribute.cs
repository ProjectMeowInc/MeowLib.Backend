using MeowLib.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.ProducesResponseTypes;

/// <summary>
/// Тип ошибок валидации. HTTP-код - 403.
/// </summary>
public class ProducesForbiddenResponseTypeAttribute : ProducesResponseTypeAttribute
{
    public ProducesForbiddenResponseTypeAttribute() : base(typeof(ValidationErrorResponse), 403) { }

    public ProducesForbiddenResponseTypeAttribute(Type type, int statusCode, string contentType,
        params string[] additionalContentTypes) : base(type, statusCode, contentType, additionalContentTypes) { }
}