namespace MeowLib.Domain.Responses.Chapter;

public class GetBookChapterResponse
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public required DateTime ReleaseDate { get; set; }
}