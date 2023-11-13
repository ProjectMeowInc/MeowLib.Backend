using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.UserFavorite;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.UserFavorite;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
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

    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateUserList([FromBody] UpdateUserListRequest input)
    {
        var userData = await GetUserDataAsync();
        var updatedUserListResult =
            await _userFavoriteService.AddOrUpdateUserListAsync(input.BookId, userData.Id, input.Status);
        if (updatedUserListResult.IsFailure)
        {
            var exception = updatedUserListResult.GetError();
            if (exception is EntityNotFoundException)
            {
                return Error("Неверно указана книга или пользователь", 400);
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

    [HttpGet("book/{bookId:int}")]
    [Authorization]
    [ProducesOkResponseType(typeof(UserFavoriteDto))]
    [ProducesNotFoundResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetUserFavoriteByBook([FromRoute] int bookId)
    {
        var userData = await GetUserDataAsync();
        var getUserFavoriteResult = await _userFavoriteService.GetUserFavoriteByBookAsync(userData.Id, bookId);
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

        return Json(foundedFavorite);
    }
}