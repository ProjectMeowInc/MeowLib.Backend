using MeowLib.Domain.Bookmark.Services;
using MeowLib.Domain.Chapter.Exceptions;
using MeowLib.Domain.User.Exceptions;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Bookmark;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.Bookmark;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер закладок пользователя.
/// </summary>
/// <param name="bookmarkService">Сервис закладок.</param>
[ApiController]
[Route("api/v1/users/bookmark")]
public class BookmarkController(IBookmarkService bookmarkService) : BaseController
{
    /// <summary>
    /// Создать или обновить существующую закладку.
    /// </summary>
    /// <param name="input">Данные для создания/обновления.</param>
    [HttpPost]
    [Authorization]
    [ProducesOkResponseType(typeof(BookmarkModel))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateOrUpdateBookmark([FromBody] CreateOrUpdateBookmarkRequest input)
    {
        var user = await GetUserDataAsync();
        var createBookmarkResult = await bookmarkService.CreateBookmarkAsync(user.Id, input.ChapterId);
        if (createBookmarkResult.IsFailure)
        {
            var exception = createBookmarkResult.GetError();
            if (exception is ChapterNotFoundException)
            {
                return Error($"Глава с Id = {input.ChapterId} не существует", 400);
            }

            if (exception is UserNotFoundException)
            {
                return Error("Неожиданная ошибка, попробуйте переавторизоваться", 400);
            }

            return ServerError();
        }

        var createdBookmark = createBookmarkResult.GetResult();
        return Json(new BookmarkModel
        {
            Id = createdBookmark.Id,
            ChapterId = createdBookmark.ChapterId
        });
    }

    /// <summary>
    /// Получение закладки пользователя по Id книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    [HttpGet("book/{bookId}")]
    [Authorization]
    [ProducesOkResponseType(typeof(BookmarkModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookmarkByBook([FromRoute] int bookId)
    {
        var userData = await GetUserDataAsync();
        var foundedBookmark = await bookmarkService.GetBookmarkByUserAndBook(userData.Id, bookId);

        if (foundedBookmark is null)
        {
            return NotFoundError($"Закладка для книги с Id = {bookId} не найдена");
        }

        return Json(new BookmarkModel
        {
            Id = foundedBookmark.Id,
            ChapterId = foundedBookmark.ChapterId
        });
    }
}