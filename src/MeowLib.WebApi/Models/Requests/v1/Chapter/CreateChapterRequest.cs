namespace MeowLib.WebApi.Models.Requests.v1.Chapter;

public class CreateChapterRequest
{
    public required string Name { get; set; }
    public required string Text { get; set; }
}