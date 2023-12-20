namespace MeowLib.WebApi.Models.Requests.v1.Team;

public class CreateTeamRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}