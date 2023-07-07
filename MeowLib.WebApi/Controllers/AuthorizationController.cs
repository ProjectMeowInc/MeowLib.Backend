using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Authorization;
using MeowLib.Domain.Requests.User;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.User;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/authorization")]
public class AuthorizationController : BaseController
{
    private readonly IUserService _userService;

    public AuthorizationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("sign-in")]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> SignIn([FromBody] SignInRequest input)
    {
        var signInResult = await _userService.SignInAsync(input.Login, input.Password);

        return signInResult.Match<ActionResult>(user => Json(user), exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            if (exception is ApiException)
            {
                return Error("Пользователь с таким логином уже занят", 400);
            }

            return ServerError();
        });
    }

    [HttpPost]
    [Route("log-in")]
    [ProducesOkResponseType(typeof(LogInResponse))]
    [ProducesForbiddenResponseType]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> LogIn([FromBody] LogInRequest input)
    {
        var logInResult = await _userService.LogIn(input.Login, input.Password, input.IsLongSession);

        return logInResult.Match<ActionResult>(tokens => Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        }), exception =>
        {
            if (exception is IncorrectCreditionalException incorrectCreditionalException)
            {
                return Error(incorrectCreditionalException.ErrorMessage, 401);
            }

            return ServerError();
        });
    }

    [HttpPost]
    [Route("update-auth")]
    [ProducesOkResponseType(typeof(LogInResponse))]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateTokens([FromBody] UpdateAuthorizationRequest input)
    {
        var loginResult = await _userService.LogInByRefreshTokenAsync(input.RefreshToken);

        return loginResult.Match<ActionResult>(tokens => Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        }), exception =>
        {
            if (exception is IncorrectCreditionalException incorrectCreditionalException)
            {
                return Error(incorrectCreditionalException.ErrorMessage, 401);
            }

            return ServerError();
        });
    }
}