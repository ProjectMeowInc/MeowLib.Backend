using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;

namespace MeowLib.Domain.DbModels.BookEntity;

public class BookEntityModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public AuthorEntityModel? Author { get; set; }
    public IEnumerable<ChapterEntityModel> Chapters { get; set; } = null!;
    public IEnumerable<TagEntityModel> Tags { get; set; } = null!;
}