using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.UserFavorite.Enums;

namespace MeowLib.Domain.UserFavorite.Entity;

public class UserFavoriteEntityModel
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Ссылка на книгу.
    /// </summary>
    public required BookEntityModel Book { get; init; }

    /// <summary>
    /// Ссылка на пользователя.
    /// </summary>
    public required UserEntityModel User { get; init; }

    /// <summary>
    /// Статус.
    /// </summary>
    public required UserFavoritesStatusEnum Status { get; set; }
}