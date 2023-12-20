using System.Text.Json.Serialization;
using MeowLib.Domain.Enums;

namespace MeowLib.WebApi.Models.Responses.v1.Team;

public class TeamMemberModel
{
    [JsonPropertyName("id")]
    public required int Id { get; init; }

    [JsonPropertyName("login")]
    public required string Login { get; init; }

    [JsonPropertyName("role")]
    public required UserTeamMemberRoleEnum Role { get; init; }
}