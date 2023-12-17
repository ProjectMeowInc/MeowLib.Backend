using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Dto.Translation;

namespace MeowLib.WebApi.Models.Responses.v1.Book;

public class GetBookResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageUrl { get; set; }
    public required AuthorDto? Author { get; set; }
    public required IEnumerable<TagDto> Tags { get; set; }
    public required IEnumerable<TranslationDto> Translations { get; set; }
}