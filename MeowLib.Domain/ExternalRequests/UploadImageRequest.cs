using Newtonsoft.Json;

namespace MeowLib.Domain.ExternalRequests;

public class UploadImageRequest
{
    [JsonProperty(PropertyName = "key")]
    public required string Key { get; init; }
    
    [JsonProperty(PropertyName = "source")]
    public required string Source { get; init; }
}