using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Exceptions.DAL;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория для работы с главами.
/// </summary>
public interface IChapterRepository
{
    
    /// <summary>
    /// Метод создаёт новую главу в базе данных.
    /// </summary>
    /// <param name="chapter">Модель главы для создания.</param>
    /// <returns>Модель созданной книги.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public Task<Result<ChapterEntityModel>> CreateAsync(ChapterEntityModel chapter);
    public Task<ChapterEntityModel?> GetByIdAsync(int chapterId);
    public Task<Option<Exception>> DeleteAsync(ChapterEntityModel chapter);
    public Task<Option<Exception>> DeleteByIdAsync(int chapterId);
    public Task<Result<ChapterEntityModel>> UpdateAsync(ChapterEntityModel chapter);
    public Result<ChapterEntityModel> UpdateBookAsync(int chapterId, BookEntityModel book);
    public Result<ChapterEntityModel> UpdateTextAsync(int chapterId, string newText);
}