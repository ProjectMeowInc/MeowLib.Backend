using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.DbModels.TeamMemberEntity;

public class TeamMemberEntityModel
{
    public int Id { get; init; }
    public required UserEntityModel User { get; init; }
    public required TeamEntityModel Team { get; init; }
    public required UserTeamMemberRoleEnum Role { get; set; }
}