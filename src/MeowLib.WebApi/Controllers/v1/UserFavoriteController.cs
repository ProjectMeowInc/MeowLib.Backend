﻿using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.UserFavorite;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.Book;
using MeowLib.WebApi.Models.Responses.v1.UserFavorite;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/users/favorite")]
public class UserFavoriteController(IUserFavoriteService userFavoriteService) : BaseController
{
    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateUserList([FromBody] UpdateUserListRequest input)
    {
        var userData = await GetUserDataAsync();
        var updatedUserListResult =
            await userFavoriteService.AddOrUpdateUserListAsync(input.BookId, userData.Id, input.Status);
        if (updatedUserListResult.IsFailure)
        {
            var exception = updatedUserListResult.GetError();
            if (exception is BookNotFoundException)
            {
                return Error("Запрашиваемая книга не найдена");
            }

            if (exception is UserNotFoundException)
            {
                return UpdateAuthorizeError();
            }
            
            return ServerError();
        }

        return EmptyResult();
    }

    [HttpGet]
    [Authorization]
    [ProducesOkResponseType(typeof(GetUserBookListResponse))]
    public async Task<ActionResult> GetUserBookList()
    {
        var userData = await GetUserDataAsync();
        var userFavorites = await userFavoriteService.GetUserFavoritesAsync(userData.Id);

        var response = new GetUserBookListResponse
        {
            Items = userFavorites
                .GroupBy(uf => uf.Status)
                .Select(d => new UserFavoriteCategoryModel
                {
                    Status = d.Key,
                    Books = d.Select(b => new BookShortModel
                    {
                        Id = b.Book.Id,
                        Name = b.Book.Name,
                        Description = b.Book.Description,
                        ImageUrl = b.Book.ImageName
                    })
                })
        };

        return Json(response);
    }

    [HttpGet("book/{bookId}")]
    [Authorization]
    [ProducesOkResponseType(typeof(UserFavoriteModel))]
    [ProducesNotFoundResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetUserFavoriteByBook([FromRoute] int bookId)
    {
        var userData = await GetUserDataAsync();
        var getUserFavoriteResult = await userFavoriteService.GetUserFavoriteByBookAsync(userData.Id, bookId);
        if (getUserFavoriteResult.IsFailure)
        {
            var exception = getUserFavoriteResult.GetError();
            if (exception is BookNotFoundException)
            {
                return Error($"Книга с Id = {bookId} не найдена", 400);
            }

            if (exception is UserNotFoundException)
            {
                return UpdateAuthorizeError();
            }

            return ServerError();
        }

        var foundedFavorite = getUserFavoriteResult.GetResult();
        if (foundedFavorite is null)
        {
            return NotFoundError("Книга отсутствует в списке пользователя");
        }
        

        return Json(new UserFavoriteModel
        {
            Id = foundedFavorite.Id,
            Status = foundedFavorite.Status,
            Book = new BookShortModel
            {
                Id = foundedFavorite.Book.Id,
                Name = foundedFavorite.Book.Name,
                Description = foundedFavorite.Book.Description,
                ImageUrl = foundedFavorite.Book.ImageUrl
            }
        });
    }
}