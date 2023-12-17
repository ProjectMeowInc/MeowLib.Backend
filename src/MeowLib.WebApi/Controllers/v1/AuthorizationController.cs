﻿using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Models.Requests.v1.Authorization;
using MeowLib.WebApi.Models.Requests.v1.User;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.User;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/authorization")]
public class AuthorizationController(IUserService userService) : BaseController
{
    [HttpPost]
    [Route("sign-in")]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> SignIn([FromBody] SignInRequest input)
    {
        var signInResult = await userService.SignInAsync(input.Login, input.Password);
        if (signInResult.IsFailure)
        {
            var exception = signInResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            if (exception is ApiException)
            {
                return Error("Пользователь с таким логином уже занят", 400);
            }

            return ServerError();
        }

        return Ok();
    }

    [HttpPost]
    [Route("log-in")]
    [ProducesOkResponseType(typeof(LogInResponse))]
    [ProducesForbiddenResponseType]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> LogIn([FromBody] LogInRequest input)
    {
        var logInResult = await userService.LogIn(input.Login, input.Password, input.IsLongSession);
        if (logInResult.IsFailure)
        {
            var exception = logInResult.GetError();
            if (exception is IncorrectCreditionalException incorrectCreditionalException)
            {
                return Error(incorrectCreditionalException.ErrorMessage, 401);
            }

            return ServerError();
        }

        var tokens = logInResult.GetResult();
        return Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }

    [HttpPost]
    [Route("update-auth")]
    [ProducesOkResponseType(typeof(LogInResponse))]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateTokens([FromBody] UpdateAuthorizationRequest input)
    {
        var loginResult = await userService.LogInByRefreshTokenAsync(input.RefreshToken);
        if (loginResult.IsFailure)
        {
            var exception = loginResult.GetError();
            if (exception is IncorrectCreditionalException incorrectCreditionalException)
            {
                return Error(incorrectCreditionalException.ErrorMessage, 401);
            }

            return ServerError();
        }

        var tokens = loginResult.GetResult();
        return Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        });
    }
}