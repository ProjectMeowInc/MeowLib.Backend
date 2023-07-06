using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IUserFavoriteRepository
{
    Task<Result<UserFavoriteEntityModel>> CreateAsync(UserFavoriteEntityModel entityModel);

    Task<UserFavoriteEntityModel?> GetByIdAsync(int id);

    Task<UserFavoriteEntityModel?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user);

    Task<bool> DeleteAsync(UserFavoriteEntityModel entityModel);

    Task<Result<UserFavoriteEntityModel>> UpdateAsync(UserFavoriteEntityModel entityModel);

    public IQueryable<UserFavoriteEntityModel> GetAll();
}