using MeowLib.Domain.Responses;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.ProducesResponseTypes;

public class ProducesServerErrorResponseTypeAttribute : ProducesResponseTypeAttribute
{
    public ProducesServerErrorResponseTypeAttribute() : base(typeof(BaseErrorResponse), 500)
    {
    }

    public ProducesServerErrorResponseTypeAttribute(Type type, int statusCode, string contentType, params string[] additionalContentTypes) : base(type, statusCode, contentType, additionalContentTypes)
    {
    }
}