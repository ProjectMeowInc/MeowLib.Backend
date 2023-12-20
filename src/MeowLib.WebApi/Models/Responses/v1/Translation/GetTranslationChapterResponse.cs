namespace MeowLib.WebApi.Models.Responses.v1.Translation;

public class GetTranslationChapterResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Text { get; init; }
    public required uint Position { get; init; }
    public required DateTime ReleaseDate { get; init; }
}