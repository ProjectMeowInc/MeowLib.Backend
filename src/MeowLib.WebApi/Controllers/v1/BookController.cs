using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Dto.Translation;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Book;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.Author;
using MeowLib.WebApi.Models.Responses.v1.Book;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/books")]
public class BookController : BaseController
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllBooksResponse))]
    public async Task<ActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        
        var response = new GetAllBooksResponse
        {
            Items = books.Select(b => new BookShortModel
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageUrl = b.ImageName
            })
        };

        return Json(response);
    }

    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType(typeof(CreateBookResponse))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest input)
    {
        var createBookResult = await _bookService.CreateBookAsync(new BookEntityModel
        {
            Name = input.Name,
            Description = input.Description,
            ImageUrl = null,
            Author = null,
            Translations = [],
            Tags = []
        });

        if (createBookResult.IsFailure)
        {
            var exception = createBookResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var createdBook = createBookResult.GetResult();
        return Json(new CreateBookResponse
        {
            CreatedId = createdBook.Id
        });
    }

    [HttpDelete("{id}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteBook([FromRoute] int id)
    {
        var deleteBookResult = await _bookService.DeleteBookByIdAsync(id);
        if (deleteBookResult.IsFailure)
        {
            return ServerError();
        }

        var bookFounded = deleteBookResult.GetResult();
        if (!bookFounded)
        {
            return NotFoundError();
        }

        return Ok();
    }

    [HttpPut("{bookId}/info")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookInfo([FromRoute] int bookId, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookResult = await _bookService.UpdateBookInfoByIdAsync(bookId, input.Name, input.Description);

        if (updateBookResult.IsFailure)
        {
            var exception = updateBookResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var updatedBook = updateBookResult.GetResult();

        if (updatedBook is null)
        {
            return Error($"Книга с Id = {bookId} не найдена");
        }

        return EmptyResult();
    }

    [HttpGet("{id}")]
    [ProducesOkResponseType(typeof(BookModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookInfo([FromRoute] int id)
    {
        var foundedBook = await _bookService.GetBookByIdAsync(id);
        if (foundedBook is null)
        {
            return NotFoundError();
        }

        return Json(new BookModel
        {
            Id = foundedBook.Id,
            Name = foundedBook.Name,
            ImageUrl = foundedBook.ImageUrl,
            Description = foundedBook.Description,
            Author = foundedBook.Author is not null
                ? new AuthorModel
                {
                    Id = foundedBook.Author.Id,
                    Name = foundedBook.Author.Name
                }
                : null,
            Tags = foundedBook.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            }),
            Translations = foundedBook.Translations.Select(t => new TranslationDto
            {
                Id = t.Id,
                Name = t.Team.Name
            })
        });
    }

    [HttpPut("{bookId}/author/{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookAuthor([FromRoute] int bookId, [FromRoute] int authorId)
    {
        var updateBookResult = await _bookService.UpdateBookAuthorAsync(bookId, authorId);
        if (updateBookResult.IsFailure)
        {
            return ServerError();
        }

        var updatedBook = updateBookResult.GetResult();
        if (updatedBook is null)
        {
            return NotFoundError();
        }

        return Json(updatedBook);
    }

    [HttpPut("{bookId}/tags")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookTags([FromRoute] int bookId, [FromBody] UpdateBookTagsRequest input)
    {
        var updateBookResult = await _bookService.UpdateBookTagsAsync(bookId, input.Tags);
        if (updateBookResult.IsFailure)
        {
            return ServerError();
        }

        var updatedBook = updateBookResult.GetResult();
        if (updatedBook is null)
        {
            return NotFoundError();
        }

        return EmptyResult();
    }

    [HttpPut("{bookId}/image")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookImage([FromRoute] int bookId, IFormFile image)
    {
        var uploadImageResult = await _bookService.UpdateBookImageAsync(bookId, image);
        if (uploadImageResult.IsFailure)
        {
            return ServerError();
        }

        var updatedBook = uploadImageResult.GetResult();
        if (updatedBook is null)
        {
            return NotFoundError($"Книга с Id = {bookId} не найдена");
        }

        return EmptyResult();
    }
}