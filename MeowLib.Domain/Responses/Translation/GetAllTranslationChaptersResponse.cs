using MeowLib.Domain.Dto.Chapter;

namespace MeowLib.Domain.Responses.Translation;

public class GetAllTranslationChaptersResponse
{
    public required IEnumerable<ChapterDto> Items { get; init; }
    public required int Count { get; init; }
}