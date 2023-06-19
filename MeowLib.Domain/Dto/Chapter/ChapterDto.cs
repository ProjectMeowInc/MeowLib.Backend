namespace MeowLib.Domain.Dto.Chapter;

public class ChapterDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime ReleaseDate { get; set; }
}