using System.Text.RegularExpressions;
using MeowLib.DAL;
using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.BookComment.Dto;
using MeowLib.Domain.BookComment.Entity;
using MeowLib.Domain.BookComment.Services;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Dto;
using MeowLib.Domain.User.Exceptions;
using MeowLib.Domain.User.Services;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис комментариев к книге.
/// </summary>
public class BookCommentService(IUserService userService, IBookService bookService, ApplicationDbContext dbContext)
    : IBookCommentService
{
    private static readonly Regex HtmlRegex = new("<[^>]*>", RegexOptions.Compiled);

    /// <summary>
    /// Метод создаёт новый комментарий.
    /// </summary>
    /// <param name="userId">Id автора комментария.</param>
    /// <param name="bookId">Id книги.</param>
    /// <param name="commentText">Текст комментария.</param>
    /// <returns>Созданный комментарий в виде <see cref="BookCommentDto" />.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    /// <exception cref="InnerException">Возникает в случае внутренних ошибок.</exception>
    public async Task<Result<BookCommentDto>> CreateNewCommentAsync(int userId, int bookId, string commentText)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<BookCommentDto>.Fail(new BookNotFoundException(bookId));
        }

        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            return Result<BookCommentDto>.Fail(new UserNotFoundException(userId));
        }

        var newComment = new BookCommentEntityModel
        {
            Text = RemoveHtml(commentText),
            PostedAt = DateTime.UtcNow,
            Author = foundedUser,
            Book = foundedBook
        };

        var createCommentResult = await dbContext.BookComments.AddAsync(newComment);
        await dbContext.SaveChangesAsync();

        var createdComment = createCommentResult.Entity;
        return new BookCommentDto
        {
            Id = createdComment.Id,
            Text = createdComment.Text,
            PostedAt = createdComment.PostedAt,
            Author = new UserDto
            {
                Id = createdComment.Author.Id,
                Login = createdComment.Author.Login,
                Role = createdComment.Author.Role
            }
        };
    }

    /// <summary>
    /// Метод возвращает список комментариев к книге.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Список комментариев в виде <see cref="BookCommentDto" />.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    public async Task<Result<IEnumerable<BookCommentDto>>> GetBookCommentsAsync(int bookId)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<IEnumerable<BookCommentDto>>.Fail(new BookNotFoundException(bookId));
        }

        var foundedComments = await dbContext
            .BookComments
            .Where(comment => comment.Book == foundedBook)
            .OrderByDescending(comment => comment.PostedAt)
            .Select(comment => new BookCommentDto
            {
                Id = comment.Id,
                Author = new UserDto
                {
                    Id = comment.Author.Id,
                    Login = comment.Author.Login,
                    Role = comment.Author.Role
                },
                PostedAt = comment.PostedAt,
                Text = comment.Text
            })
            .ToListAsync();

        return foundedComments;
    }

    private static string RemoveHtml(string str)
    {
        return HtmlRegex.Replace(str, string.Empty);
    }
}