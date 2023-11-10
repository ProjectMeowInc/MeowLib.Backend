using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.ProducesResponseTypes;

public class ProducesOkResponseTypeAttribute : ProducesResponseTypeAttribute
{
    public ProducesOkResponseTypeAttribute() : base(typeof(void), 200)
    {
    }

    public ProducesOkResponseTypeAttribute(Type type) : base(type, 200)
    {
    }

    public ProducesOkResponseTypeAttribute(Type type, int statusCode, string contentType,
        params string[] additionalContentTypes) : base(type, statusCode, contentType, additionalContentTypes)
    {
    }
}