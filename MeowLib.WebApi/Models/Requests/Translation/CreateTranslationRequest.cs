using System.Text.Json.Serialization;

namespace MeowLib.WebApi.Models.Requests.Translation;

public class CreateTranslationRequest
{
    [JsonPropertyName("bookId")]
    public required int BookId { get; init; }
    [JsonPropertyName("teamId")]
    public required int TeamId { get; init; }
}