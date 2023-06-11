using LanguageExt.Common;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Exceptions.DAL;

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
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки БД.</exception>
    Task<Result<ChapterEntityModel>> CreateChapterAsync(string name, string text, int bookId);
}