namespace MeowLib.WebApi.Models.Responses.v2.Translation;

public class VolumeModel
{
    public required uint Number { get; init; }
    public required List<ChapterModel> Chapters { get; init; }
}