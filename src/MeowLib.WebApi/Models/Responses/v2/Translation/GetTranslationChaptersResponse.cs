namespace MeowLib.WebApi.Models.Responses.v2.Translation;

public class GetTranslationChaptersResponse
{
    public required List<ChapterModel> Items { get; init; }
    public required int Count { get; init; }
}