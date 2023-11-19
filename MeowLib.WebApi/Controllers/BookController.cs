using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Dto.Translation;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.Book;
using MeowLib.WebApi.Models.Responses;
using MeowLib.WebApi.Models.Responses.Book;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.Controllers;

[Route("api/books")]
public class BookController : BaseController
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookService _bookService;

    public BookController(IBookService bookService, IBookRepository bookRepository)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllBooksResponse))]
    public async Task<ActionResult> GetAllBooks()
    {
        var response = new GetAllBooksResponse
        {
            Items = await _bookRepository.GetAll().Select(book => new BookDto
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                ImageName = book.ImageUrl
            }).ToListAsync()
        };

        return Json(response);
    }

    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType(typeof(BookEntityModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest input)
    {
        var createBookResult = await _bookService.CreateBookAsync(new CreateBookEntityModel
        {
            Name = input.Name,
            Description = input.Description
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
        return Json(createdBook);
    }

    [HttpDelete("{id:int}")]
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

    [HttpPut("{bookId:int}/info")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookInfo([FromRoute] int bookId, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookResult = await _bookService.UpdateBookInfoByIdAsync(bookId, new UpdateBookEntityModel
        {
            Name = input.Name,
            Description = input.Description
        });

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

    [HttpGet("{id:int}")]
    [ProducesOkResponseType(typeof(GetBookResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookInfo([FromRoute] int id)
    {
        var foundedBook = await _bookService.GetBookByIdAsync(id);
        if (foundedBook is null)
        {
            return NotFoundError();
        }

        return Json(new GetBookResponse
        {
            Id = foundedBook.Id,
            Name = foundedBook.Name,
            ImageUrl = foundedBook.ImageUrl,
            Description = foundedBook.Description,
            Author = foundedBook.Author is not null
                ? new AuthorDto
                {
                    Id = foundedBook.Author.Id,
                    Name = foundedBook.Author.Name
                }
                : null,
            Tags = foundedBook.Tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name
            }),
            Translations = foundedBook.Translations.Select(t => new TranslationDto
            {
                Id = t.Id,
                Name = t.Team.Name
            })
        });
    }

    [HttpPut("{bookId:int}/author/{authorId:int}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateBookAuthor([FromRoute] int bookId, [FromRoute] int authorId)
    {
        var updateBookResult = await _bookService.UpdateBookAuthorAsync(bookId, authorId);
        if (updateBookResult.IsFailure)
        {
            var exception = updateBookResult.GetError();
            if (exception is EntityNotFoundException)
            {
                return Error($"Автор с Id = {authorId} не найден", 400);
            }

            return ServerError();
        }

        var updatedBook = updateBookResult.GetResult();
        if (updatedBook is null)
        {
            return NotFoundError();
        }

        return Json(updatedBook);
    }

    [HttpPut("{bookId:int}/tags")]
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

    [HttpPut("{bookId:int}/image")]
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