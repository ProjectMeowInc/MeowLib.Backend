using MeowLib.Domain.Dto.Author;

namespace MeowLib.Domain.Dto.Book;

public class BookDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string? ImageName { get; init; }
    public required AuthorDto? Author { get; init; }
}