using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;

namespace MeowLIb.WebApi.Services.Interface;

public interface IUserFavoriteService
{
    /// <summary>
    /// Метод обновляет книгу в списке пользователя.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="status">Статус для обновления</param>
    /// <returns>Обновлённую книгу в списке пользователя.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга или пользователь не были найдены.</exception>
    Task<Result<UserFavoriteEntityModel>> AddOrUpdateUserListAsync(int bookId, int userId,
        UserFavoritesStatusEnum status);

    Task<List<UserFavoriteDto>> GetUserFavorites(int userId);
}