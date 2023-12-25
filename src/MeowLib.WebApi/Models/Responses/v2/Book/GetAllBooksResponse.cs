namespace MeowLib.WebApi.Models.Responses.v2.Book;

public class GetAllBooksResponse
{
    public required List<ShortBookModel> Items { get; init; }
}