using MeowLib.Domain.Enums;

namespace MeowLib.WebApi.Models.Requests.Team;

public class SetUserTeamRoleRequest
{
    public required UserTeamMemberRoleEnum NewRole { get; set; }
}