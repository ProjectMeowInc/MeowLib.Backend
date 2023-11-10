using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;

namespace MeowLIb.WebApi.Services.Interface;

/// <summary>
/// Абстракция сервиса комментариев книг.
/// </summary>
public interface IBookCommentService
{
    /// <summary>
    /// Метод создаёт новый комментарий.
    /// </summary>
    /// <param name="userId">Id автора комментария.</param>
    /// <param name="bookId">Id книги.</param>
    /// <param name="commentText">Текст комментария.</param>
    /// <returns>Созданный комментарий в виде <see cref="BookCommentDto"/>.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    /// <exception cref="InnerException">Возникает в случае внутренних ошибок.</exception>
    Task<Result<BookCommentDto>> CreateNewCommentAsync(int userId, int bookId, string commentText);

    /// <summary>
    /// Метод возвращает список комментариев к книге.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Список комментариев в виде <see cref="BookCommentDto"/>.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    Task<Result<IEnumerable<BookCommentDto>>> GetBookCommentsAsync(int bookId);
}