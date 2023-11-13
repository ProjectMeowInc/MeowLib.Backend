using MeowLib.Domain.Dto.Bookmark;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.Bookmark;
using MeowLib.Domain.Responses;
using MeowLIb.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[ApiController]
[Route("api/users/bookmark")]
public class BookmarkController : BaseController
{
    private readonly IBookmarkService _bookmarkService;

    public BookmarkController(IBookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    [HttpPost]
    [Authorization]
    [ProducesOkResponseType(typeof(BookmarkDto))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateOrUpdateBookmark([FromBody] CreateOrUpdateBookmarkRequest input)
    {
        var user = await GetUserDataAsync();
        var createBookmarkResult = await _bookmarkService.CreateBookmarkAsync(user.Id, input.ChapterId);
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
        return Json(createdBookmark);
    }

    [HttpGet("book/{bookId}")]
    [Authorization]
    [ProducesOkResponseType(typeof(BookmarkDto))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookmarkByBook([FromRoute] int bookId)
    {
        var userData = await GetUserDataAsync();
        var foundedBookmark = await _bookmarkService.GetBookmarkByUserAndBook(userData.Id, bookId);

        if (foundedBookmark is null)
        {
            return NotFoundError($"Закладка для книги с Id = {bookId} не найдена");
        }

        return Json(foundedBookmark);
    }
}