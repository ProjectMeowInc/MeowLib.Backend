using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.BookComment;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.BookComment;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[ApiController]
[Route("api/books")]
public class BookCommentController : BaseController
{
    private readonly IBookCommentService _bookCommentService;
    
    public BookCommentController(IBookCommentService bookCommentService)
    {
        _bookCommentService = bookCommentService;
    }

    [HttpGet("{bookId:int}/comments")]
    [ProducesOkResponseType(typeof(GetBookCommentsResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookComments([FromRoute] int bookId)
    {
        var getBookCommentsResult = await _bookCommentService.GetBookCommentsAsync(bookId);
        
        return getBookCommentsResult.Match<ActionResult>(bookComments 
            => Json(new GetBookCommentsResponse
        {
            BookId = bookId,
            Items = bookComments
        }), exception =>
        {
            if (exception is BookNotFoundException)
            {
                return NotFoundError($"Книга с Id = {bookId} не найдена");
            }

            return ServerError();
        });
    }

    [HttpPost("{bookId:int}/comments"), Authorization]
    [ProducesOkResponseType(typeof(BookCommentDto))]
    [ProducesResponseType(400,  Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> PostComment([FromRoute] int bookId, [FromBody] PostCommentRequest input)
    {
        var userData = await GetUserDataAsync();

        var createNewCommentResult = await _bookCommentService.CreateNewCommentAsync(userData.Id, bookId, input.Text);
        return createNewCommentResult.Match<ActionResult>(createdComment => Json(createdComment),
            exception =>
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