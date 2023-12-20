namespace MeowLib.WebApi.Models.Responses.v1.Book;

public class GetAllBooksResponse
{
    public required IEnumerable<BookShortModel> Items { get; init; }
}