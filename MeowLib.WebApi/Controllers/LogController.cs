using MeowLib.Domain.Requests.Log;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/logs")]
public class LogController : BaseController
{
    private readonly IFrontEndLogService _frontEndLogService;
    
    public LogController(IFrontEndLogService frontEndLogService)
    {
        _frontEndLogService = frontEndLogService;
    }

    [HttpPost, Authorization]
    [ProducesOkResponseType]
    public async Task<ActionResult> SendLog([FromBody] LogRequest input)
    {
        var userInfo = await GetUserDataAsync();
        await _frontEndLogService.LogAsync(userInfo.Login, input.ErrorLog);
        return Empty();
    }
}