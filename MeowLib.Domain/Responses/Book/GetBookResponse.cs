using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.Tag;

namespace MeowLib.Domain.Responses.Book;

public class GetBookResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required AuthorDto Author { get; set; }
    public required IEnumerable<TagDto> Tags { get; set; }
}