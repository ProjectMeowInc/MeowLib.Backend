using MeowLib.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.ProducesResponseTypes;

public class ProducesNotFoundResponseTypeAttribute : ProducesResponseTypeAttribute
{
    public ProducesNotFoundResponseTypeAttribute() : base(typeof(BaseErrorResponse), 404) { }

    public ProducesNotFoundResponseTypeAttribute(Type type, int statusCode, string contentType,
        params string[] additionalContentTypes) : base(type, statusCode, contentType, additionalContentTypes) { }
}