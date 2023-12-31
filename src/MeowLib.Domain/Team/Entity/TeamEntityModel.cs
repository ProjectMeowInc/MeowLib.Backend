using MeowLib.Domain.TeamMember.Entity;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.Team.Entity;

public class TeamEntityModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required UserEntityModel Owner { get; init; }
    public required List<TeamMemberEntityModel> Members { get; init; }
}