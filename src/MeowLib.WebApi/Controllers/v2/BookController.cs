using MeowLib.Domain.Book.Services;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
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
}