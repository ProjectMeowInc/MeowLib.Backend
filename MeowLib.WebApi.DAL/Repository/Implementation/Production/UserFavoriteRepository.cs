using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с книгами пользователей.
/// </summary>
public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст БД.</param>
    public UserFavoriteRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод создаёт новую книгу в списке пользователя.
    /// </summary>
    /// <param name="entityModel">Модель для создания.</param>
    /// <returns>Созданную книгу в списке пользователя.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<UserFavoriteEntityModel>> CreateAsync(UserFavoriteEntityModel entityModel)
    {
        var createdUserFavoritesEntry = await _applicationDbContext.UsersFavorite.AddAsync(entityModel);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(UserFavoriteEntityModel), DbSavingTypesEnum.Create);
            return new Result<UserFavoriteEntityModel>(dbSavingException);
        }

        return createdUserFavoritesEntry.Entity;
    }

    /// <summary>
    /// Метод получает книгу пользователя по Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>Найденную книгу или null, если её нет.</returns>
    public async Task<UserFavoriteEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Id == id);
    }

    /// <summary>
    /// Метод получает книгу в списке пользователя по книгу и пользователю.
    /// </summary>
    /// <param name="book">Книга.</param>
    /// <param name="user">Пользователь.</param>
    /// <returns>Найденную книгу или null, если её нет.</returns>
    public async Task<UserFavoriteEntityModel?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Book == book && uf.User == user);
    }

    /// <summary>
    /// Метод удаляет книгу в списке пользователя.
    /// </summary>
    /// <param name="entityModel">Книга в списке пользователя для удаления</param>
    /// <returns>True - в случае успеха, иначе - false.</returns>
    public async Task<bool> DeleteAsync(UserFavoriteEntityModel entityModel)
    {
        _applicationDbContext.UsersFavorite.Remove(entityModel);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Метод обновляет книгу в спике пользователя.
    /// </summary>
    /// <param name="entityModel">Книга в списке пользователя для обновления.</param>
    /// <returns>Обновлённую модель книги</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<UserFavoriteEntityModel>> UpdateAsync(UserFavoriteEntityModel entityModel)
    {
        var updateUserFavoriteEntry = _applicationDbContext.UsersFavorite.Update(entityModel);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(UserFavoriteEntityModel), DbSavingTypesEnum.Update);
            return new Result<UserFavoriteEntityModel>(dbSavingException);
        }

        return updateUserFavoriteEntry.Entity;
    }

    /// <summary>
    /// Метод получает все книги в списке пользователя в виде <see cref="IQueryable{T}"/>
    /// </summary>
    /// <returns></returns>
    public IQueryable<UserFavoriteEntityModel> GetAll() => _applicationDbContext.UsersFavorite.AsQueryable();
}