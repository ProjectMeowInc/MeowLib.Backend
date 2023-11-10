using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Result;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория для работы с книгами пользователей.
/// </summary>
public interface IUserFavoriteRepository
{
    /// <summary>
    /// Метод создаёт новую книгу в списке пользователя.
    /// </summary>
    /// <param name="entityModel">Модель для создания.</param>
    /// <returns>Созданную книгу в списке пользователя.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<UserFavoriteEntityModel>> CreateAsync(UserFavoriteEntityModel entityModel);

    /// <summary>
    /// Метод получает книгу пользователя по Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>Найденную книгу или null, если её нет.</returns>
    Task<UserFavoriteEntityModel?> GetByIdAsync(int id);

    /// <summary>
    /// Метод получает книгу в списке пользователя по книгу и пользователю.
    /// </summary>
    /// <param name="book">Книга.</param>
    /// <param name="user">Пользователь.</param>
    /// <returns>Найденную книгу или null, если её нет.</returns>
    Task<UserFavoriteEntityModel?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user);

    /// <summary>
    /// Метод удаляет книгу в списке пользователя.
    /// </summary>
    /// <param name="entityModel">Книга в списке пользователя для удаления</param>
    /// <returns>True - в случае успеха, иначе - false.</returns>
    Task<bool> DeleteAsync(UserFavoriteEntityModel entityModel);

    /// <summary>
    /// Метод обновляет книгу в спике пользователя.
    /// </summary>
    /// <param name="entityModel">Книга в списке пользователя для обновления.</param>
    /// <returns>Обновлённую модель книги</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<UserFavoriteEntityModel>> UpdateAsync(UserFavoriteEntityModel entityModel);

    /// <summary>
    /// Метод получает все книги в списке пользователя в виде <see cref="IQueryable{T}"/>
    /// </summary>
    /// <returns></returns>
    IQueryable<UserFavoriteEntityModel> GetAll();
}