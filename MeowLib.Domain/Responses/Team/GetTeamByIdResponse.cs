using System.Text.Json.Serialization;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Responses.Team;

public class GetTeamByIdResponse
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("name")]
    public required string Name { get; init; }
    [JsonPropertyName("description")]
    public required string Description { get; init; }
    [JsonPropertyName("members")]
    public required IEnumerable<TeamMember> Members { get; init; }
}

public class TeamMember
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }
    [JsonPropertyName("login")]
    public required string Login { get; init; }
    [JsonPropertyName("role")]
    public required UserTeamMemberRoleEnum Role { get; init; }
}