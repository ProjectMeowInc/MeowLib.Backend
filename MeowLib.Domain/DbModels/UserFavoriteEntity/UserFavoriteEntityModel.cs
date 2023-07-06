using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;

namespace MeowLib.Domain.DbModels.UserFavoriteEntity;

public class UserFavoriteEntityModel
{
    /// <summary>
    /// Id.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Ссылка на книгу.
    /// </summary>
    public required BookEntityModel Book { get; set; }
    
    /// <summary>
    /// Ссылка на пользователя.
    /// </summary>
    public required UserEntityModel User { get; set; }
    
    /// <summary>
    /// Статус.
    /// </summary>
    public required UserFavoritesStatusEnum Status { get; set; }
}