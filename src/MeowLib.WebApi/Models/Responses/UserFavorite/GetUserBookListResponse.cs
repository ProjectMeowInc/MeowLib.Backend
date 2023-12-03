using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Enums;

namespace MeowLib.WebApi.Models.Responses.UserFavorite;

public class GetUserBookListResponse
{
    public required IEnumerable<UserFavoriteCategory> Items { get; init; }
}

public class UserFavoriteCategory
{
    public required UserFavoritesStatusEnum Status { get; init; }
    public required IEnumerable<BookDto> Books { get; init; }
}