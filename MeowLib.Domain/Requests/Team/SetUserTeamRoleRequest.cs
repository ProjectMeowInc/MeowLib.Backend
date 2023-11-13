using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Requests.Team;

public class SetUserTeamRoleRequest
{
    public required UserTeamMemberRoleEnum NewRole { get; set; }
}