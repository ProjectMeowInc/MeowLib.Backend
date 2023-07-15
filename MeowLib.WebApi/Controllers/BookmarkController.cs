﻿using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.Bookmark;
using MeowLib.Domain.Responses;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
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

    [HttpPost, Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateOrUpdateBookmark([FromBody] CreateOrUpdateBookmarkRequest input)
    {
        var user = await GetUserDataAsync();
        var createBookmarkResult = await _bookmarkService.CreateBookmarkAsync(user.Id, input.ChapterId);

        return createBookmarkResult.Match<ActionResult>(createdBookmark => Json(createdBookmark), exception =>
        {
            if (exception is ChapterNotFoundException)
            {
                return Error($"Глава с Id = {input.ChapterId} не существует", 400);
            }

            if (exception is UserNotFoundException)
            {
                return Error("Неожиданная ошибка, попробуйте переавторизоваться", 400);
            }

            return ServerError();
        });
    }
}