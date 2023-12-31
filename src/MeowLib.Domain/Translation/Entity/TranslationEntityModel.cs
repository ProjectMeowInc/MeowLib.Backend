using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.Team.Entity;

namespace MeowLib.Domain.Translation.Entity;

public class TranslationEntityModel
{
    public int Id { get; init; }
    public required BookEntityModel Book { get; init; }
    public required TeamEntityModel Team { get; init; }
    public required IEnumerable<ChapterEntityModel> Chapters { get; init; }
}