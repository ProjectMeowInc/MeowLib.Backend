using MeowLib.Domain.Enums;
using MeowLib.WebApi.Models.Responses.v1.Book;

namespace MeowLib.WebApi.Models.Responses.v1.UserFavorite;

public class UserFavoriteCategoryModel
{
    public required UserFavoritesStatusEnum Status { get; init; }
    public required IEnumerable<BookShortModel> Books { get; init; }
}