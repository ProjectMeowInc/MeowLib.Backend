using MeowLib.Domain.Author.Dto;

namespace MeowLib.Domain.Book.Dto;

public class BookDto
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required string? ImageName { get; init; }
    public required PeopleDto? Author { get; init; }
}