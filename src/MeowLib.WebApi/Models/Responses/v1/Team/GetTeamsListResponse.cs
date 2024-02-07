namespace MeowLib.WebApi.Models.Responses.v1.Team;

public class GetTeamsListResponse
{
    public required IEnumerable<TeamModel> Items { get; init; }
}