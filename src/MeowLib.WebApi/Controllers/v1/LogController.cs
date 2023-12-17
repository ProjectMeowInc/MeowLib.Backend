using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Log;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/logs")]
public class LogController : BaseController
{
    private readonly IFrontEndLogService _frontEndLogService;

    public LogController(IFrontEndLogService frontEndLogService)
    {
        _frontEndLogService = frontEndLogService;
    }

    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    public async Task<ActionResult> SendLog([FromBody] LogRequest input)
    {
        var userInfo = await GetUserDataAsync();
        await _frontEndLogService.LogAsync(userInfo.Login, input.ErrorLog);
        return EmptyResult();
    }
}