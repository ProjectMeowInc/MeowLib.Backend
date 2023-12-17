using MeowLib.WebApi.Models.Responses.v1;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.ProducesResponseTypes;

/// <summary>
/// Тип ошибок пользователя. HTTP-код - 400.
/// </summary>
public class ProducesUserErrorResponseTypeAttribute : ProducesResponseTypeAttribute
{
    public ProducesUserErrorResponseTypeAttribute() : base(typeof(BaseErrorResponse), 400) { }
}