using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IUserFavoriteRepository
{
    Task<Result<UserFavoriteEntity>> CreateAsync(UserFavoriteEntity entity);

    Task<UserFavoriteEntity?> GetByIdAsync(int id);

    Task<UserFavoriteEntity?> GetByBookAndUserAsync(BookEntityModel book, UserEntityModel user);

    Task<bool> DeleteAsync(UserFavoriteEntity entity);

    Task<Option<Exception>> UpdateAsync(UserFavoriteEntity entity);

    public IQueryable<UserFavoriteEntity> GetAll();
}