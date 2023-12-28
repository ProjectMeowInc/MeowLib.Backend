using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.Dto.UserFavorite;

public class UserFavoriteDto
{
    public required BookDto Book { get; init; }
    public required UserFavoritesStatusEnum Status { get; init; }
}