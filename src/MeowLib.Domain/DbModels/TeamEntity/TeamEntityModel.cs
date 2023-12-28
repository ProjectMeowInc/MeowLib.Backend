using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.DbModels.UserEntity;

namespace MeowLib.Domain.DbModels.TeamEntity;

public class TeamEntityModel
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required UserEntityModel Owner { get; init; }
    public required List<TeamMemberEntityModel> Members { get; init; }
}