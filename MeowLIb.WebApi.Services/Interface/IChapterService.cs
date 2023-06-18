using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;

namespace MeowLIb.WebApi.Services.Interface;

public interface IChapterService
{
    /// <summary>
    /// Метод создаёт новую главу.
    /// </summary>
    /// <param name="name">Название главы.</param>
    /// <param name="text">Текст главы.</param>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель созданной главы.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки БД.</exception>
    Task<Result<ChapterEntityModel>> CreateChapterAsync(string name, string text, int bookId);

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Новый текст главы.</param>
    /// <returns>Модель обновлённой главы.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<ChapterEntityModel>> UpdateChapterTextAsync(int chapterId, string newText);

    /// <summary>
    /// Метод возвращает главы книги в виде <see cref="ChapterDto"/>
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель главы в виде <see cref="ChapterDto"/></returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга с заданым Id не была найдена.</exception>
    Task<Result<IEnumerable<ChapterDto>>> GetAllBookChapters(int bookId);

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Option<Exception>> DeleteChapterAsync(int chapterId);

    /// <summary>
    /// Метод возвращает главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Модель главы, если была найдена.</returns>
    Task<ChapterEntityModel?> GetChapterByIdAsync(int chapterId);
}