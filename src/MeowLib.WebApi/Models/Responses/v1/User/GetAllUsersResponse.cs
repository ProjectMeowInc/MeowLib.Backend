namespace MeowLib.WebApi.Models.Responses.v1.User;

public class GetAllUsersResponse
{
    public required IEnumerable<UserModel> Items { get; init; }
}