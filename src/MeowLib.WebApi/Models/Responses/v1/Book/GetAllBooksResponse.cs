using MeowLib.Domain.Dto.Book;

namespace MeowLib.WebApi.Models.Responses.v1.Book;

public class GetAllBooksResponse
{
    public required IEnumerable<BookDto> Items { get; init; }
}