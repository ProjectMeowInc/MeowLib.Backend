using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Result;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IBookmarkRepository
{
    Task<Result<BookmarkEntityModel>> CreateAsync(BookmarkEntityModel entity);

    Task<BookmarkEntityModel?> GetByIdAsync(int id);

    public IQueryable<BookmarkEntityModel> GetAll();

    Task<Result<bool>> DeleteByIdAsync(int id);

    Task<BookmarkEntityModel?> GetByUserAndChapterAsync(UserEntityModel user, ChapterEntityModel chapter);

    Task<Result<BookmarkEntityModel>> UpdateAsync(BookmarkEntityModel entity);
}