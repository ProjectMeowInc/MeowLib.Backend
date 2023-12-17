using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Result;

namespace MeowLib.DAL.Repository.Interfaces;

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
    public Task<Result<ChapterEntityModel>> CreateAsync(ChapterEntityModel chapter);

    public Task<ChapterEntityModel?> GetByIdAsync(int chapterId);

    /// <summary>
    /// Метод удаляет главу.
    /// </summary>
    /// <param name="chapter">Глава для удаления.</param>
    /// <returns>Ошибку, если она есть.</returns>
    public Task<Result> DeleteAsync(ChapterEntityModel chapter);

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть. Так же возращает ошибки метода <see cref="DeleteAsync" />.</returns>
    public Task<Result> DeleteByIdAsync(int chapterId);

    public Task<Result<ChapterEntityModel>> UpdateAsync(ChapterEntityModel chapter);
    public Task<Result<ChapterEntityModel>> UpdateBookAsync(int chapterId, BookEntityModel book);

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Текст для обновления.</param>
    /// <returns>Модель главы.</returns>
    public Task<Result<ChapterEntityModel>> UpdateTextAsync(int chapterId, string newText);

    public IQueryable<ChapterEntityModel> GetAll();
}