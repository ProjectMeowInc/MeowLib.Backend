using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.File.Services;
using MeowLib.Domain.Shared.Exceptions;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Models.Requests.v2.Book;
using MeowLib.WebApi.Models.Responses.v1.Tag;
using MeowLib.WebApi.Models.Responses.v1.Translation;
using MeowLib.WebApi.Models.Responses.v2.Author;
using MeowLib.WebApi.Models.Responses.v2.Book;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v2;

[Route("api/v2/books")]
public class BookController(IBookService bookService, IFileService fileService, ILogger<BookController> logger)
    : BaseController
{
    /// <summary>
    /// Получение всех книг.
    /// </summary>
    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllBooksResponse))]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await bookService.GetAllBooksAsync();
        return Ok(new GetAllBooksResponse
        {
            Items = books.Select(b => new ShortBookModel
                {
                    Id = b.Id,
                    Name = b.Name,
                    Description = b.Description,
                    ImageUrl = b.ImageName,
                    Author = b.Author != null
                        ? new AuthorShortModel
                        {
                            Id = b.Author.Id,
                            Name = b.Author.Name
                        }
                        : null
                })
                .ToList()
        });
    }

    /// <summary>
    /// Получение информации о книге.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    [HttpGet("{bookId}")]
    [ProducesOkResponseType(typeof(GetBookResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetBookById([FromRoute] int bookId)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return NotFoundError();
        }

        return Ok(new GetBookResponse
        {
            Id = foundedBook.Id,
            Name = foundedBook.Name,
            Description = foundedBook.Description,
            ImageUrl = foundedBook.Image?.FileSystemName,
            Peoples = foundedBook.Peoples.Select(p => new PeopleWithBookRoleModel
            {
                Id = p.People.Id,
                Name = p.People.Name,
                Role = p.Role
            }),
            Tags = foundedBook.Tags.Select(t => new TagModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            }),
            Translations = foundedBook.Translations.Select(t => new TranslationModel
            {
                Id = t.Id,
                Name = t.Team.Name
            })
        });
    }

    /// <summary>
    /// Создание новой книги.
    /// </summary>
    /// <param name="payload">Данные для создания</param>
    [HttpPost]
    [ProducesOkResponseType(typeof(CreateBookRequest))]
    [ProducesUserErrorResponseType]
    [ProducesForbiddenResponseType]
    public async Task<IActionResult> CreateBook([FromBody] CreateBookRequest payload)
    {
        var foundedImage = await fileService.GetFileByIdAsync(payload.ImageId);
        if (foundedImage is null)
        {
            return Error("Изображение не найдено", 400);
        }

        var result = await bookService.CreateBookAsync(new BookEntityModel
        {
            Name = payload.Name,
            Description = payload.Description,
            Image = foundedImage,
            Peoples = [],
            Translations = [],
            Tags = [],
            Characters = []
        });

        if (result.IsFailure)
        {
            var exception = result.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            logger.LogError("Неизвестная ошибка создания книги: {exception}", exception);
            return ServerError();
        }

        var book = result.GetResult();
        return Ok(new CreateBookResponse
        {
            CreatedId = book.Id
        });
    }
}