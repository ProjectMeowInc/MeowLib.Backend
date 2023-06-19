using MeowLib.Domain.Dto.Chapter;

namespace MeowLib.Domain.Responses.Chapter;

public class GetAllBookChaptersResponse
{
    public required IEnumerable<ChapterDto> Items { get; set; }
}