using System.Text.Json.Serialization;

namespace MeowLib.WebApi.Models.Requests.Translation;

public class AddChapterToTranslationRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("text")]
    public required string Text { get; init; }
    [JsonPropertyName("position")]
    public required uint Position { get; init; }
}