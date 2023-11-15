using MeowLib.DAL;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TranslationEntity;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class TranslationService(ApplicationDbContext dbContext) : ITranslationService
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

        await dbContext.Chapters.AddAsync(newChapter);
        await dbContext.SaveChangesAsync();
        
        return Result.Ok();
    }
}