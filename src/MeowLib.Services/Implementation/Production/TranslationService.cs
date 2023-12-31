using MeowLib.DAL;
using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Chapter.Dto;
using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.Chapter.Exceptions;
using MeowLib.Domain.Notification.Services;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Team.Entity;
using MeowLib.Domain.Translation.Entity;
using MeowLib.Domain.Translation.Exceptions;
using MeowLib.Domain.Translation.Services;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class TranslationService(ApplicationDbContext dbContext, INotificationService notificationService)
    : ITranslationService
{
    /// <summary>
    /// Метод создаёт перевод для заданной книги.
    /// </summary>
    /// <param name="book">Книги для добавления перевода.</param>
    /// <param name="team">Комманда, с которой будет связан перевод.</param>
    /// <returns>Результат создания перевода.</returns>
    /// <exception cref="TeamAlreadyTranslateBookException">Заданная комманда уже имеет перевод для этой книги.</exception>
    public async Task<Result> CreateTranslationAsync(BookEntityModel book, TeamEntityModel team)
    {
        if (book.Translations.Any(t => t.Team.Id == team.Id))
        {
            return Result.Fail(new TeamAlreadyTranslateBookException(team.Id, book.Id));
        }

        await dbContext.Translations.AddAsync(new TranslationEntityModel
        {
            Book = book,
            Team = team,
            Chapters = new List<ChapterEntityModel>()
        });
        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    /// <summary>
    /// Метод возвращает список глав перевода.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <returns>Список глав перевода</returns>
    /// <exception cref="TranslationNotFoundException">Возникает в случае, если перевод с заданным Id не был найден</exception>
    public async Task<Result<List<ChapterDto>>> GetTranslationChaptersAsync(int translationId)
    {
        var foundedTranslation = await dbContext.Translations
            .FirstOrDefaultAsync(t => t.Id == translationId);

        if (foundedTranslation is null)
        {
            return Result<List<ChapterDto>>.Fail(new TranslationNotFoundException(translationId));
        }

        var translationChapters = await dbContext.Chapters.Where(c => c.Translation.Id == translationId)
            .OrderBy(c => c.Position)
            .ThenBy(c => c.ReleaseDate)
            .Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                ReleaseDate = c.ReleaseDate,
                Position = c.Position
            })
            .ToListAsync();

        return translationChapters;
    }

    /// <summary>
    /// Метод возвращает главу книги по переводу и её положению в нём.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="position">Позиция в переводе.</param>
    /// <returns>Искомую главу или null, если она не найдена.</returns>
    public async Task<ChapterEntityModel?> GetChapterByTranslationAndPositionAsync(int translationId, int position)
    {
        var foundedChapter = await dbContext.Chapters
            .Where(t => t.Translation.Id == translationId)
            .FirstOrDefaultAsync(t => t.Position == position);

        return foundedChapter;
    }

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
    public async Task<Result> AddChapterAsync(int translationId, string name, string text, uint position)
    {
        var foundedTranslation = await dbContext.Translations
            .Include(translationEntityModel => translationEntityModel.Chapters)
            .Include(translationEntityModel => translationEntityModel.Book)
            .FirstOrDefaultAsync(t => t.Id == translationId);

        if (foundedTranslation is null)
        {
            return Result.Fail(new TranslationNotFoundException(translationId));
        }

        if (foundedTranslation.Chapters.Any(c => c.Position == position))
        {
            return Result.Fail(new ChapterPositionAlreadyTaken(position));
        }

        var newChapter = new ChapterEntityModel
        {
            Name = name,
            Text = text,
            ReleaseDate = DateTime.UtcNow,
            Position = position,
            Translation = foundedTranslation
        };

        var newChapterEntry = await dbContext.Chapters.AddAsync(newChapter);
        await dbContext.SaveChangesAsync();

        //todo: move to worker?
        var sendNotificationResult =
            await notificationService.SendNotificationToBookSubscribersAsync(foundedTranslation.Book.Id,
                newChapterEntry.Entity.Name);

        if (sendNotificationResult.IsFailure)
        {
            return Result.Fail(new InnerException(sendNotificationResult.GetError().Message));
        }

        return Result.Ok();
    }

    /// <summary>
    /// Метод возвращает перевод по его Id
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <returns>Найденный перевод или null</returns>
    public async Task<TranslationEntityModel?> GetTranslationByIdAsync(int translationId)
    {
        return await dbContext.Translations.FirstOrDefaultAsync(t => t.Id == translationId);
    }
}