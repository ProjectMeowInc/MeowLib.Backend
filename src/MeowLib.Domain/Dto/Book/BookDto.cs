using MeowLib.Domain.Dto.Author;

namespace MeowLib.Domain.Dto.Book;

public class BookDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageName { get; set; }
    public required AuthorDto? Author { get; init; }
}