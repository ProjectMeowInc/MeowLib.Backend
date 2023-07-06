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

    public async Task<UserFavoriteEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Id == id);
    }

    public async Task<UserFavoriteEntityModel?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user)
    {
        return await _applicationDbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.Book == book && uf.User == user);
    }

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

    public IQueryable<UserFavoriteEntityModel> GetAll() => _applicationDbContext.UsersFavorite.AsQueryable();
}