using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TeamEntity;

namespace MeowLib.Domain.DbModels.TranslationEntity;

public class TranslationEntityModel
{
    public int Id { get; init; }
    public required BookEntityModel Book { get; init; }
    public required TeamEntityModel Team { get; init; }
    public required IEnumerable<ChapterEntityModel> Chapters { get; init; }
}