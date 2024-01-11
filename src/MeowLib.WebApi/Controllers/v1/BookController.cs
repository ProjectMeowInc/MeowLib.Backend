using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.BookPeople.Enums;
using MeowLib.Domain.People.Exceptions;
using MeowLib.Domain.Shared.Exceptions;
using MeowLib.Domain.Tag.Dto;
using MeowLib.Domain.User.Enums;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Book;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.Author;
using MeowLib.WebApi.Models.Responses.v1.Book;
using MeowLib.WebApi.Models.Responses.v1.Translation;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер книг.
/// </summary>
/// <param name="bookService">Сервис книг.</param>
[Route("api/v1/books")]
public class BookController(IBookService bookService, ILogger<BookController> logger) : BaseController
{
    /// <summary>
    /// [DEPRECATED] Получение всех книг.
    /// </summary>
    [HttpGet]
    [DeprecatedMethod(10, 2, 2024)]
    [ProducesOkResponseType(typeof(GetAllBooksResponse))]
    public async Task<ActionResult> GetAllBooks()
    {
        var books = await bookService.GetAllBooksAsync();

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

    /// <summary>
    /// Создание новой книги.
    /// </summary>
    /// <param name="input">Данные для создания книги.</param>
    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType(typeof(CreateBookResponse))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest input)
    {
        var createBookResult = await bookService.CreateBookAsync(new BookEntityModel
        {
            Name = input.Name,
            Description = input.Description,
            Image = null,
            Translations = [],
            Tags = [],
            Peoples = []
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

    /// <summary>
    /// Удаление книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    [HttpDelete("{bookId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteBook([FromRoute] int bookId)
    {
        var deleteBookResult = await bookService.DeleteBookByIdAsync(bookId);
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

    /// <summary>
    /// Обновление информации книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="input">Данные для обновления.</param>
    [HttpPut("{bookId}/info")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> UpdateBookInfo([FromRoute] int bookId, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookResult = await bookService.UpdateBookInfoByIdAsync(bookId, input.Name, input.Description);

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

    /// <summary>
    /// [DEPRECATED] Получение информации о книге.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    [HttpGet("{bookId}")]
    [ProducesOkResponseType(typeof(BookModel))]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> GetBookInfo([FromRoute] int bookId)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return NotFoundError();
        }

        return Json(new BookModel
        {
            Id = foundedBook.Id,
            Name = foundedBook.Name,
            ImageUrl = foundedBook.Image?.FileSystemName,
            Description = foundedBook.Description,
            Author = foundedBook.Peoples
                .Where(p => p.Role == BookPeopleRoleEnum.Author)
                .Select(p => new AuthorModel
                {
                    Id = p.People.Id,
                    Name = p.People.Name
                })
                .FirstOrDefault(),
            Tags = foundedBook.Tags.Select(t => new TagDto
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
    /// [DEPRECATED] Обновление автора книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="authorId">Id автора.</param>
    [HttpPut("{bookId}/author/{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> UpdateBookAuthor([FromRoute] int bookId, [FromRoute] int authorId)
    {
        var updateBookResult = await bookService.UpdateBookAuthorAsync(bookId, authorId);
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

    /// <summary>
    /// Обновление тегов книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="input">Данные для обновления.</param>
    [HttpPut("{bookId}/tags")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> UpdateBookTags([FromRoute] int bookId, [FromBody] UpdateBookTagsRequest input)
    {
        var updateBookResult = await bookService.UpdateBookTagsAsync(bookId, input.Tags);
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

    /// <summary>
    /// Обновление картинки книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="image">Картинка для обновления.</param>
    [HttpPut("{bookId}/image")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> UpdateBookImage([FromRoute] int bookId, IFormFile image)
    {
        var uploadImageResult = await bookService.UpdateBookImageAsync(bookId, image);
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

    /// <summary>
    /// Добавления человека к книге.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="payload">Данные для добавления.</param>
    [HttpPost("{bookId}/people")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Moderator })]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> AddPeople([FromRoute] int bookId, [FromBody] AddPeopleRequest payload)
    {
        var addPeopleResult = await bookService.AddPeopleToBookAsync(bookId, payload.PeopleId, payload.Role);
        if (addPeopleResult.IsFailure)
        {
            var exception = addPeopleResult.GetError();
            if (exception is BookNotFoundException)
            {
                return NotFoundError("Книга не найдена");
            }

            if (exception is PeopleNotFoundException)
            {
                return NotFoundError("Человек не найден");
            }

            if (exception is PeopleAlreadyAttachedException)
            {
                return Error("Человек уже прикреплён к книге", 400);
            }

            logger.LogError("Ошибка добавления человека к книге: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    /// <summary>
    /// Удаление человека из книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="peopleId">Id человека.</param>
    [HttpDelete("{bookId}/people/{peopleId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Moderator })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> RemovePeople([FromRoute] int bookId, [FromRoute] int peopleId)
    {
        var removePeopleResult = await bookService.DeletePeopleFromBookAsync(peopleId, bookId);
        if (removePeopleResult.IsFailure)
        {
            var exception = removePeopleResult.GetError();
            if (exception is PeopleNotFoundException)
            {
                return NotFoundError("Человек не найден");
            }

            if (exception is BookNotFoundException)
            {
                return NotFoundError("Книга не найдена");
            }

            logger.LogError("Ошибка удаления человека из книги: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }
}