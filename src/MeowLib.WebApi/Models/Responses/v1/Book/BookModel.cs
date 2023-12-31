using MeowLib.Domain.Tag.Dto;
using MeowLib.Domain.Translation.Dto;
using MeowLib.WebApi.Models.Responses.v1.Author;

namespace MeowLib.WebApi.Models.Responses.v1.Book;

public class BookModel
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string? ImageUrl { get; set; }
    public required AuthorModel? Author { get; set; }
    public required IEnumerable<TagDto> Tags { get; set; }
    public required IEnumerable<TranslationDto> Translations { get; set; }
}