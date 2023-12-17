using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.BookComment;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.BookComment;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[ApiController]
[Route("api/v1/books")]
public class BookCommentController(IBookCommentService bookCommentService) : BaseController
{
    [HttpGet("{bookId}/comments")]
    [ProducesOkResponseType(typeof(GetBookCommentsResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookComments([FromRoute] int bookId)
    {
        var getBookCommentsResult = await bookCommentService.GetBookCommentsAsync(bookId);
        if (getBookCommentsResult.IsFailure)
        {
            var exception = getBookCommentsResult.GetError();
            if (exception is BookNotFoundException)
            {
                return NotFoundError($"Книга с Id = {bookId} не найдена");
            }

            return ServerError();
        }

        var bookComments = getBookCommentsResult.GetResult();
        return Json(new GetBookCommentsResponse
        {
            BookId = bookId,
            Items = bookComments.Select(bk => new BookCommentModel
            {
                Id = bk.Id,
                Text = bk.Text,
                PostedAt = bk.PostedAt,
                Author = bk.Author
            })
        });
    }

    [HttpPost("{bookId}/comments")]
    [Authorization]
    [ProducesOkResponseType(typeof(BookCommentDto))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> PostComment([FromRoute] int bookId, [FromBody] PostCommentRequest input)
    {
        var userData = await GetUserDataAsync();

        var createNewCommentResult = await bookCommentService.CreateNewCommentAsync(userData.Id, bookId, input.Text);
        if (createNewCommentResult.IsFailure)
        {
            var exception = createNewCommentResult.GetError();
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

        var createdComment = createNewCommentResult.GetResult();
        return Json(createdComment);
    }
}