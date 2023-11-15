using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.TeamEntity;
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
}