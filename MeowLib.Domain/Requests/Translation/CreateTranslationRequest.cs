using System.Text.Json.Serialization;

namespace MeowLib.Domain.Requests.Translation;

public class CreateTranslationRequest
{
    [JsonPropertyName("bookId")]
    public required int BookId { get; init; }
    [JsonPropertyName("teamId")]
    public required int TeamId { get; init; }
}