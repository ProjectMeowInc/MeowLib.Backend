﻿using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.User;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.User;
using MeowLib.WebApi.Abstractions;
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
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
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
    [ProducesResponseType(200, Type = typeof(LogInResponse))]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> LogIn([FromBody] LogInRequest input)
    {
        var logInResult = await _userService.LogIn(input.Login, input.Password, input.IsLongSession);

        return logInResult.Match<ActionResult>(tokens => Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        }), exception =>
        {
            if (exception is IncorrectCreditionalException)
            {
                return Error("Неверный логин или пароль", 401);
            }

            return ServerError();
        });
    }

    [HttpPost]
    [Route("update-auth")]
    [ProducesResponseType(200, Type = typeof(LogInResponse))]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateTokens([FromHeader(Name = "RefreshToken")] string refreshToken)
    {
        var loginResult = await _userService.LogInByRefreshTokenAsync(refreshToken);

        return loginResult.Match<ActionResult>(tokens => Json(new LogInResponse
        {
            AccessToken = tokens.accessToken,
            RefreshToken = tokens.refreshToken
        }), exception =>
        {
            if (exception is IncorrectCreditionalException)
            {
                return Error("Неверный токен обновления", 401);
            }

            return ServerError();
        });
    }
}