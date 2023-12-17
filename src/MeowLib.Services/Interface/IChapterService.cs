using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Result;

namespace MeowLib.Services.Interface;

/// <summary>
/// Абстракция сервиса для работы с главами.
/// </summary>
public interface IChapterService
{
    /// <summary>
    /// Метод создаёт новую главу.
    /// </summary>
    /// <param name="name">Название главы.</param>
    /// <param name="text">Текст главы.</param>
    /// <param name="translationId">Id перевода.</param>
    /// <returns>Модель созданной главы.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="TranslationNotFoundException">Возникает в случае, если перевод не был найден.</exception>
    Task<Result<ChapterEntityModel>> CreateChapterAsync(string name, string text, int translationId);

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Новый текст главы.</param>
    /// <returns>Модель обновлённой главы.</returns>
    Task<Result<ChapterEntityModel>> UpdateChapterTextAsync(int chapterId, string newText);

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть.</returns>
    Task<Result> DeleteChapterAsync(int chapterId);
}