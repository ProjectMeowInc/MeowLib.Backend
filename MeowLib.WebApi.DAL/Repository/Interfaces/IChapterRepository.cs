using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IChapterRepository
{
    public Task<Result<ChapterEntityModel>> CreateAsync(ChapterEntityModel chapter);
    public Task<ChapterEntityModel?> GetByIdAsync(int chapterId);
    public Task<Option<Exception>> DeleteAsync(ChapterEntityModel chapter);
    public Task<Option<Exception>> DeleteByIdAsync(int chapterId);
    public Task<Result<ChapterEntityModel>> UpdateAsync(ChapterEntityModel chapter);
    public Result<ChapterEntityModel> UpdateBookAsync(int chapterId, BookEntityModel book);
    public Result<ChapterEntityModel> UpdateTextAsync(int chapterId, string newText);
}