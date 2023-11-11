namespace MeowLib.Domain.Requests.Team;

public class CreateTeamRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}