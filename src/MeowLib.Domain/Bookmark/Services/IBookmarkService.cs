using MeowLib.Domain.Bookmark.Dto;
using MeowLib.Domain.Chapter.Exceptions;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Exceptions;

namespace MeowLib.Domain.Bookmark.Services;

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