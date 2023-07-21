﻿using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.UserFavorite;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.UserFavorite;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/users/favorite")]
public class UserFavoriteController : BaseController
{
    private readonly IUserFavoriteService _userFavoriteService;

    public UserFavoriteController(IUserFavoriteService userFavoriteService)
    {
        _userFavoriteService = userFavoriteService;
    }

    [HttpPost, Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateUserList([FromBody] UpdateUserListRequest input)
    {
        var userData = await GetUserDataAsync();
        var updatedUserListResult = await _userFavoriteService.AddOrUpdateUserListAsync(input.BookId, userData.Id, input.Status);

        return updatedUserListResult.Match<ActionResult>(_ => EmptyResult(), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error("Неверно указана книга или пользователь", 400);
            }

            return ServerError();
        });
    }

    [HttpGet, Authorization]
    [ProducesOkResponseType(typeof(GetUserBookListResponse))]
    public async Task<ActionResult> GetUserBookList()
    {
        var userData = await GetUserDataAsync();
        var userFavorites = await _userFavoriteService.GetUserFavoritesAsync(userData.Id);

        var response = new GetUserBookListResponse
        {
            Items = userFavorites
                .GroupBy(uf => uf.Status)
                .Select(d => new UserFavoriteCategory
                {
                    Status = d.Key,
                    Books = d.Select(b => b.Book)
                })
        };
        
        return Json(response);
    }

    [HttpGet("book/{bookId:int}"), Authorization]
    [ProducesOkResponseType(typeof(UserFavoriteDto))]
    [ProducesNotFoundResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetUserFavoriteByBook([FromRoute] int bookId)
    {
        var userData = await GetUserDataAsync();
        var getUserFavoriteResult = await _userFavoriteService.GetUserFavoriteByBookAsync(userData.Id, bookId);
        return getUserFavoriteResult.Match<ActionResult>(foundedFavorite =>
        {
            if (foundedFavorite is null)
            {
                return NotFoundError("Книга отсутствует в списке пользователя");
            }

            return Json(foundedFavorite);
        }, exception =>
        {
            if (exception is BookNotFoundException)
            {
                return Error($"Книга с Id = {bookId} не найдена", 400);
            }

            if (exception is UserNotFoundException)
            {
                return UpdateAuthorizeResult();
            }
            
            return ServerError();
        });
    }
}