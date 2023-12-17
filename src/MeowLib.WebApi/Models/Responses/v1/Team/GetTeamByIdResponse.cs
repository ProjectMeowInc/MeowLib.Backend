using System.Text.Json.Serialization;

namespace MeowLib.WebApi.Models.Responses.v1.Team;

public class GetTeamByIdResponse
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("members")]
    public required IEnumerable<TeamMemberModel> Members { get; init; }
}