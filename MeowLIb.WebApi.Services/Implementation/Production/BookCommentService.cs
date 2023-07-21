﻿using System.Text.RegularExpressions;
using AutoMapper;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.Dto.BookComment;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Book;
using MeowLib.Domain.Exceptions.User;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис комментариев к книге.
/// </summary>
public class BookCommentService : IBookCommentService
{
    private static Regex _htmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
    
    private readonly IBookCommentRepository _bookCommentRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="bookCommentRepository">Репозиторий комментариев к книге.</param>
    /// <param name="bookRepository">Репозиторий книг.</param>
    /// <param name="userRepository">Репозиторий пользователя.</param>
    /// <param name="mapper">Автомаппер.</param>
    public BookCommentService(IBookCommentRepository bookCommentRepository, IBookRepository bookRepository, 
        IUserRepository userRepository, IMapper mapper)
    {
        _bookCommentRepository = bookCommentRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _mapper = mapper;
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
            var bookNotFoundException = new BookNotFoundException(bookId);
            return new Result<BookCommentDto>(bookNotFoundException);
        }

        var foundedUser = await _userRepository.GetByIdAsync(userId);
        if (foundedUser is null)
        {
            var userNotFoundException = new UserNotFoundException(userId);
            return new Result<BookCommentDto>(userNotFoundException);
        }

        var newComment = new BookCommentEntityModel
        {
            Text = RemoveHtml(commentText),
            PostedAt = DateTime.UtcNow,
            Author = foundedUser,
            Book = foundedBook
        };

        var createCommentResult = await _bookCommentRepository.CreateAsync(newComment);
        return createCommentResult.Match<Result<BookCommentDto>>(createdComment => 
            _mapper.Map<BookCommentEntityModel, BookCommentDto>(createdComment), _ =>
        {
            // TODO: ADD LOGS
            
            var innerException = new InnerException("Ошибка в БД");
            return new Result<BookCommentDto>(innerException);
        });
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
            var bookNotFoundException = new BookNotFoundException(bookId);
            return new Result<IEnumerable<BookCommentDto>>(bookNotFoundException);
        }

        var foundedComments = await _bookCommentRepository
            .GetAll()
            .Where(comment => comment.Book == foundedBook)
            .OrderBy(comment => comment.PostedAt)
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
        return _htmlRegex.Replace(str, String.Empty);
    }
}