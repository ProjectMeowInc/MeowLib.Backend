using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Requests.UserFavorite;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
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
    public async Task<ActionResult> UpdateUserList([FromBody] UpdateUserListRequest input)
    {
        var userData = await GetUserDataAsync();
        var updateUserListResult = await _userFavoriteService.AddOrUpdateUserListAsync(input.BookId, userData.Id, input.Status);

        return updateUserListResult.Match<ActionResult>(_ => Empty(), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error("Неверно указана книга или пользователь", 400);
            }

            return ServerError();
        });
    }
}