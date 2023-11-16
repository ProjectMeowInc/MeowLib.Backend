using System.Text.RegularExpressions;
using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис комментариев к книге.
/// </summary>
public class BookCommentService : IBookCommentService
{
    private static Regex _htmlRegex = new("<[^>]*>", RegexOptions.Compiled);

    private readonly IBookCommentRepository _bookCommentRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="bookCommentRepository">Репозиторий комментариев к книге.</param>
    /// <param name="bookRepository">Репозиторий книг.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    public BookCommentService(IBookCommentRepository bookCommentRepository, IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        _bookCommentRepository = bookCommentRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
    }

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
    public async Task<Result<BookCommentDto>> CreateNewCommentAsync(int userId, int bookId, string commentText)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<BookCommentDto>.Fail(new BookNotFoundException(bookId));
        }

        var foundedUser = await _userRepository.GetByIdAsync(userId);
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

        var createCommentResult = await _bookCommentRepository.CreateAsync(newComment);
        if (createCommentResult.IsFailure)
        {
            return Result<BookCommentDto>.Fail(createCommentResult.GetError());
        }

        var createdComment = createCommentResult.GetResult();
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
    /// <returns>Список комментариев в виде <see cref="BookCommentDto"/>.</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    public async Task<Result<IEnumerable<BookCommentDto>>> GetBookCommentsAsync(int bookId)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<IEnumerable<BookCommentDto>>.Fail(new BookNotFoundException(bookId));
        }

        var foundedComments = await _bookCommentRepository
            .GetAll()
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
        return _htmlRegex.Replace(str, string.Empty);
    }
}