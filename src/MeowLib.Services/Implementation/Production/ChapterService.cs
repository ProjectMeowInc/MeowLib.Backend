using MeowLib.DAL;
using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.Chapter.Exceptions;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Translation.Exceptions;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с главами.
/// </summary>
public class ChapterService(ApplicationDbContext dbContext)
    : IChapterService
{
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

        var foundedTranslation = await dbContext.Translations.FirstOrDefaultAsync(t => t.Id == translationId);
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

        var entry = await dbContext.Chapters.AddAsync(newChapter);
        await dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Новый текст главы.</param>
    /// <returns>Модель обновлённой главы или null если она не найдена.</returns>
    public async Task<Result<ChapterEntityModel?>> UpdateChapterTextAsync(int chapterId, string newText)
    {
        var foundedChapter = await GetChapterByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            return Result<ChapterEntityModel?>.Ok(null);
        }

        foundedChapter.Text = newText;

        dbContext.Chapters.Update(foundedChapter);
        await dbContext.SaveChangesAsync();

        return foundedChapter;
    }

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть.</returns>
    public async Task<Result> DeleteChapterAsync(int chapterId)
    {
        var foundedChapter = await GetChapterByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            return Result.Fail(new ChapterNotFoundException(chapterId));
        }

        dbContext.Chapters.Remove(foundedChapter);
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    public Task<ChapterEntityModel?> GetChapterByIdAsync(int chapterId)
    {
        return dbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
    }
}