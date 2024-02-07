namespace MeowLib.WebApi.Models.Requests.v2.Translation;

public class UploadChapterRequest
{
    public required string Name { get; init; }
    public required string Text { get; init; }
    public required uint Position { get; init; }
    public required uint Volume { get; init; }
}