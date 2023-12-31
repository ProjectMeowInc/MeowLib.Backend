using MeowLib.Domain.Team.Entity;
using MeowLib.Domain.TeamMember.Enums;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.TeamMember.Entity;

public class TeamMemberEntityModel
{
    public int Id { get; init; }
    public required UserEntityModel User { get; init; }
    public required TeamEntityModel Team { get; init; }
    public required UserTeamMemberRoleEnum Role { get; set; }
}