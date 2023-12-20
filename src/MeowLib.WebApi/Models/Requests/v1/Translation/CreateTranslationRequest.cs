using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace MeowLib.WebApi.Models.Requests.v1.Translation;

public class CreateTranslationRequest
{
    [JsonPropertyName("bookId")]
    public required int BookId { get; init; }

    [JsonPropertyName("teamId")]
    public required int TeamId { get; init; }
}