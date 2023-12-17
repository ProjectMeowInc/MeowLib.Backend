namespace MeowLib.WebApi.Models.Responses.v1.UserFavorite;

public class GetUserBookListResponse
{
    public required IEnumerable<UserFavoriteCategoryModel> Items { get; init; }
}