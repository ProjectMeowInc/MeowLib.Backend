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

public class UserFavoriteRepository : IUserFavoriteRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserFavoriteRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<UserFavoriteEntity>> CreateAsync(UserFavoriteEntity entity)
    {
        var createdUserFavoritesEntry = await _applicationDbContext.UsersFavorite.AddAsync(entity);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(UserFavoriteEntity), DbSavingTypesEnum.Create);
            return new Result<UserFavoriteEntity>(dbSavingException);
        }

        return createdUserFavoritesEntry.Entity;
    }

    public async Task<UserFavoriteEntity?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Id == id);
    }

    public async Task<UserFavoriteEntity?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Book == book && uf.User == user);
    }

    public async Task<bool> DeleteAsync(UserFavoriteEntity entity)
    {
        _applicationDbContext.UsersFavorite.Remove(entity);

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

    public async Task<Option<Exception>> UpdateAsync(UserFavoriteEntity entity)
    {
        _applicationDbContext.UsersFavorite.Update(entity);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(UserFavoriteEntity), DbSavingTypesEnum.Update);
            return Option<Exception>.Some(dbSavingException);
        }

        return Option<Exception>.None;
    }

    public IQueryable<UserFavoriteEntity> GetAll() => _applicationDbContext.UsersFavorite.AsQueryable();
}