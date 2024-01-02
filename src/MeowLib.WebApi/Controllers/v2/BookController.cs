using MeowLib.Domain.Book.Services;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Responses.v1.Tag;
using MeowLib.WebApi.Models.Responses.v1.Translation;
using MeowLib.WebApi.Models.Responses.v2.Author;
using MeowLib.WebApi.Models.Responses.v2.Book;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v2;

[Route("api/v2/books")]
public class BookController(IBookService bookService) : BaseController
{
    /// <summary>
    /// Получение всех книг.
    /// </summary>
    [HttpGet]
    [UnstableMethod]
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
}