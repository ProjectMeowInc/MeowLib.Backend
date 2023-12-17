using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Log;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/logs")]
public class LogController(IFrontEndLogService frontEndLogService) : BaseController
{
    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    public async Task<ActionResult> SendLog([FromBody] LogRequest input)
    {
        var userInfo = await GetUserDataAsync();
        await frontEndLogService.LogAsync(userInfo.Login, input.ErrorLog);
        return EmptyResult();
    }
}