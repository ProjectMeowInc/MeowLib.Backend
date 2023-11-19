using MeowLib.DAL;
using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Models;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с главами.
/// </summary>
public class ChapterService : IChapterService
{
    private readonly IBookService _bookService;
    private readonly IChapterRepository _chapterRepository;
    private readonly ApplicationDbContext _dbContext;

    public ChapterService(IChapterRepository chapterRepository, IBookService bookService,
        ApplicationDbContext dbContext)
    {
        _chapterRepository = chapterRepository;
        _bookService = bookService;
        _dbContext = dbContext;
    }

    public async Task<Result<ChapterEntityModel>> CreateChapterAsync(string name, string text, int translationId)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (name.Length < 1 || name.Length > 50)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = "Name",
                Message = "Имя не может быть меньше 1 или больше 50 символов"
            });
        }

        if (text.Length > 10000)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = "Text",
                Message = "Текст главы не может быть больше 10000 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return Result<ChapterEntityModel>.Fail(validationException);
        }

        var foundedTranslation = await _dbContext.Translations.FirstOrDefaultAsync(t => t.Id == translationId);
        if (foundedTranslation is null)
        {
            return Result<ChapterEntityModel>.Fail(new TranslationNotFoundException(translationId));
        }

        var newChapter = new ChapterEntityModel
        {
            Name = name,
            Text = text,
            ReleaseDate = DateTime.UtcNow,
            Translation = foundedTranslation,
            Position = 1
        };

        var createChapterResult = await _chapterRepository.CreateAsync(newChapter);
        if (createChapterResult.IsFailure)
        {
            return Result<ChapterEntityModel>.Fail(createChapterResult.GetError());
        }

        return createChapterResult.GetResult();
    }

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Новый текст главы.</param>
    /// <returns>Модель обновлённой главы.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<ChapterEntityModel>> UpdateChapterTextAsync(int chapterId, string newText)
    {
        var updateResult = await _chapterRepository.UpdateTextAsync(chapterId, newText);
        if (updateResult.IsFailure)
        {
            return Result<ChapterEntityModel>.Fail(updateResult.GetError());
        }

        return updateResult.GetResult();
    }

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result> DeleteChapterAsync(int chapterId)
    {
        var deleteResult = await _chapterRepository.DeleteByIdAsync(chapterId);
        if (deleteResult.IsFailure)
        {
            return Result.Fail(deleteResult.GetError());
        }

        return Result.Ok();
    }

    /// <summary>
    /// Метод возвращает главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Модель главы, если была найдена.</returns>
    public async Task<ChapterEntityModel?> GetChapterByIdAsync(int chapterId)
    {
        return await _chapterRepository.GetByIdAsync(chapterId);
    }
}