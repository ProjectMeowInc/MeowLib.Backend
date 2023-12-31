using MeowLib.Domain.Dto.Bookmark;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Shared.Result;

namespace MeowLib.Services.Interface;

public interface IBookmarkService
{
    /// <summary>
    /// Метод создаёт новую (если её нет) или обновляет старую закладку книги пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Созданную/обновлённую закладку в виде <see cref="BookmarkDto" />.</returns>
    /// <exception cref="ChapterNotFoundException">Возникает в случае, если не была найдена глава.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если не был найден пользователь.</exception>
    /// <exception cref="InnerException">Возникает в случае внутренних проблем.</exception>
    Task<Result<BookmarkDto>> CreateBookmarkAsync(int userId, int chapterId);

    Task<BookmarkDto?> GetBookmarkByUserAndBook(int userId, int bookId);
}