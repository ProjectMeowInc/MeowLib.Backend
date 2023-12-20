using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.DbModels.UserEntity;

namespace MeowLib.Domain.DbModels.TeamEntity;

public class TeamEntityModel
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required UserEntityModel Owner { get; init; }
    public required List<TeamMemberEntityModel> Members { get; init; }
}