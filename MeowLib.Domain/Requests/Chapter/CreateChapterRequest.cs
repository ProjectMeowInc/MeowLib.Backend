namespace MeowLib.Domain.Requests.Chapter;

public class CreateChapterRequest
{
    public required string Name { get; set; }
    public required string Text { get; set; }
}