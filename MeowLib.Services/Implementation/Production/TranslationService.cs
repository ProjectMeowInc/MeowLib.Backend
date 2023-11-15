using MeowLib.DAL;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TranslationEntity;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;

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
}