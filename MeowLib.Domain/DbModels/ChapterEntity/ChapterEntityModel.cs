using MeowLib.Domain.DbModels.BookEntity;

namespace MeowLib.Domain.DbModels.ChapterEntity;

public class ChapterEntityModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Text { get; set; }
    public required DateTime ReleaseDate { get; set; }
    public required BookEntityModel Book { get; set; }
}