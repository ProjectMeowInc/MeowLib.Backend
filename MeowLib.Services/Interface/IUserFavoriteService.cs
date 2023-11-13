using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.User;

namespace MeowLib.Services.Interface;

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
    Task<MeowLib.Domain.Result.Result<UserFavoriteEntityModel>> AddOrUpdateUserListAsync(int bookId, int userId,
        UserFavoritesStatusEnum status);

    /// <summary>
    /// Метод получает список избранных книг пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns>Список избранныъ книг пользователя.</returns>
    Task<List<UserFavoriteDto>> GetUserFavoritesAsync(int userId);

    /// <summary>
    /// Метод получает книгу в списке пользователя по её Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Информацию о книге, если она была найдена. Иначе - null</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    Task<MeowLib.Domain.Result.Result<UserFavoriteDto?>> GetUserFavoriteByBookAsync(int userId, int bookId);
}