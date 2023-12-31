using MeowLib.Domain.Book.Dto;
using MeowLib.Domain.UserFavorite.Enums;

namespace MeowLib.Domain.UserFavorite.Dto;

public class UserFavoriteDto
{
    public required BookDto Book { get; init; }
    public required UserFavoritesStatusEnum Status { get; init; }
}