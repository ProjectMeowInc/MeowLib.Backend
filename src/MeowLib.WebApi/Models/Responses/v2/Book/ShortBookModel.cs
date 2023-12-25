using MeowLib.WebApi.Models.Responses.v2.Author;

namespace MeowLib.WebApi.Models.Responses.v2.Book;

public class ShortBookModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string? ImageUrl { get; init; }
    public required AuthorShortModel? Author { get; init; }
}