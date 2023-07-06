using System.Text.Json.Serialization;

namespace MeowLib.Domain.ExternalResponses;

public class UploadImageResponse
{
    [JsonPropertyName("status_code")]
    public required int StatusCode { get; init; }
    
    [JsonPropertyName("url")]
    public string? Url { get; init; }
}