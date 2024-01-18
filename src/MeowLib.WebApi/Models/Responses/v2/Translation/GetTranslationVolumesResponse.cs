namespace MeowLib.WebApi.Models.Responses.v2.Translation;

public class GetTranslationVolumesResponse
{
    public required IEnumerable<VolumeModel> Volumes { get; init; }
}