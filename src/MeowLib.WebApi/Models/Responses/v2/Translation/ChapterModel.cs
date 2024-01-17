namespace MeowLib.WebApi.Models.Responses.v2.Translation;

public class ChapterModel
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required DateTime ReleaseDate { get; init; }
    public required uint Volume { get; init; }
    public required uint Position { get; init; }
}