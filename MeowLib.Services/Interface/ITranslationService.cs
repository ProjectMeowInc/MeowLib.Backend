using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TranslationEntity;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Result;

namespace MeowLib.Services.Interface;

public interface ITranslationService
{
    /// <summary>
    /// Метод создаёт перевод для заданной книги.
    /// </summary>
    /// <param name="book">Книги для добавления перевода.</param>
    /// <param name="team">Комманда, с которой будет связан перевод.</param>
    /// <returns>Результат создания перевода.</returns>
    /// <exception cref="TeamAlreadyTranslateBookException">Заданная комманда уже имеет перевод для этой книги.</exception>
    Task<Result> CreateTranslationAsync(BookEntityModel book, TeamEntityModel team);

    /// <summary>
    /// Метод возвращает список глав перевода.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <returns>Список глав перевода</returns>
    /// <exception cref="TranslationNotFoundException">Возникает в случае, если перевод с заданным Id не был найден</exception>
    Task<Result<List<ChapterDto>>> GetTranslationChaptersAsync(int translationId);

    /// <summary>
    /// Метод возвращает главу книги по переводу и её положению в нём.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="position">Позиция в переводе.</param>
    /// <returns>Искомую главу или null, если она не найдена.</returns>
    Task<ChapterEntityModel?> GetChapterByTranslationAndPositionAsync(int translationId,
        int position);

    /// <summary>
    /// Метод добавляет главу в перевод.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="name">Название главы.</param>
    /// <param name="text">Контент главы.</param>
    /// <param name="position">Пизиция в списке глав.</param>
    /// <returns>Результат добавления главы.</returns>
    /// <exception cref="TranslationNotFoundException">Возникает в случае, если перевод не был найден.</exception>
    /// <exception cref="ChapterPositionAlreadyTaken">Возникает в случае, если заданная позиция уже занята.</exception>
    Task<Result> AddChapterAsync(int translationId, string name, string text, uint position);

    /// <summary>
    /// Метод возвращает перевод по его Id
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <returns>Найденный перевод или null</returns>
    Task<TranslationEntityModel?> GetTranslationByIdAsync(int translationId);
}