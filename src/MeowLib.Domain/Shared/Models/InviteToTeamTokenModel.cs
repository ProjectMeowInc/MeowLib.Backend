namespace MeowLib.Domain.Models;

public class InviteToTeamTokenModel
{
    public required int UserId { get; init; }
    public required int TeamId { get; init; }
    public required DateTime InviteExpiredAt { get; init; }
}