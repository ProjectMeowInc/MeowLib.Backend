using MeowLib.Domain.Dto.Book;

namespace MeowLib.Domain.Responses.Book;

public class GetAllBooksResponse
{
    public required IEnumerable<BookDto> Items { get; init; }
}