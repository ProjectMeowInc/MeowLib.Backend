using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.User;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.User;
using MeowLib.WebApi.Abstractions;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("sign-in")]
    [ProducesResponseType(200, Type = typeof(UserDto))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    public async Task<ActionResult> SignIn([FromBody] SignInRequest input)
    {
        try
        {
            var _ = await _userService.SignInAsync(input.Login, input.Password);
            return Ok();
        }
        catch (ValidationException validationException)
        {
            var responseData = new ValidationErrorResponse(validationException.ValidationErrors);
            return Json(responseData, 400);
        }
        catch (ApiException apiException)
        {
            var responseData = new BaseErrorResponse(apiException.ErrorMessage);
            return Json(responseData, 403);
        }
    }

    [HttpPost]
    [Route("log-in")]
    [ProducesResponseType(200, Type = typeof(LogInResponse))]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> LogIn([FromBody] LogInRequest input)
    {
        try
        {
            var jwtToken = await _userService.LogIn(input.Login, input.Password);
            var responseData = new LogInResponse(jwtToken);
            return Json(responseData);
        }
        catch (ApiException apiException)
        {
            var responseData = new BaseErrorResponse(apiException.ErrorMessage);
            return Json(responseData, 401);
        }
    }
}