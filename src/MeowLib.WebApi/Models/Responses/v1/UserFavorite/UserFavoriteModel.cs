using MeowLib.Domain.Enums;
using MeowLib.WebApi.Models.Responses.v1.Book;

namespace MeowLib.WebApi.Models.Responses.v1.UserFavorite;

public class UserFavoriteModel
{
    public required int Id { get; init; }
    public required UserFavoritesStatusEnum Status { get; init; }
    public required BookShortModel Book { get; init; }
}