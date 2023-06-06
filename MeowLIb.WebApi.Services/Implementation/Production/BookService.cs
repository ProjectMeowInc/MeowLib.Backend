using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с книгами.
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="bookRepository">Репозиторий книг.</param>
    /// <param name="authorRepository">Репозиторий авторов.</param>
    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Метод создаёт новую книгу.
    /// </summary>
    /// <param name="createBookEntityModel">Данные для создания книги.</param>
    /// <returns>Модель созданной книги.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel>> CreateBookAsync(CreateBookEntityModel createBookEntityModel)
    {
        var validationErrors = new List<ValidationErrorModel>();

        var inputName = createBookEntityModel.Name.Trim();
        var inputDescription = createBookEntityModel.Description.Trim();

        if (inputName.Length < 5)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(createBookEntityModel.Name),
                Message = "Имя не может быть короче 5 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(nameof(BookService), validationErrors);
            return new Result<BookEntityModel>(validationException);
        }

        try
        {
            return await _bookRepository.CreateAsync(new BookEntityModel
            {
                Name = inputName,
                Description = inputDescription,
                Author = null
            });
        }
        catch (DbSavingException)
        {
            // TODO: Add logs
            var apiException = new ApiException("Внутреяя ошибка сервера");
            return new Result<BookEntityModel>(apiException);
        }
    }

    /// <summary>
    /// Метод обновляет основную информацию о книге по её Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="updateBookEntityModel">Информация для обновления.</param>
    /// <returns>Обновлённая модель книги или null если книга не найдена.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookInfoByIdAsync(int bookId, UpdateBookEntityModel updateBookEntityModel)
    {
        var inputName = updateBookEntityModel.Name?.Trim() ?? null;
        var inputDescription = updateBookEntityModel.Name?.Trim() ?? null;

        var validationErrors = new List<ValidationErrorModel>();
            
        if (inputName is not null && inputName.Length < 5)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(updateBookEntityModel.Name),
                Message = "Имя книги не может быть меньше 5 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(nameof(BookService), validationErrors);
            return new Result<BookEntityModel?>(validationException);
        }

        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            return null;
        }

        foundedBook.Name = inputName ?? foundedBook.Name;
        foundedBook.Description = inputDescription ?? foundedBook.Description;

        try
        {
            var updatedBook = await _bookRepository.UpdateAsync(foundedBook);
            return updatedBook;
        }
        catch (DbSavingException)
        {
            // TODO: Add logs
            var apiException = new ApiException("Внутреняя ошибка сервера.");
            return new Result<BookEntityModel?>(apiException);
        }
    }
    
    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="bookId">Id книги для обновления.</param>
    /// <param name="authorId">Id автора для обновления.</param>
    /// <returns>Обновлённую модель книги при удачном обновления, null - если книга не была найдена.</returns>
    /// <exception cref="ApiException">Возникает в случае если автор не был найден или при ошибке сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookAuthorAsync(int bookId, int authorId)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            return null;
        }

        var foundedAuthor = await _authorRepository.GetByIdAsync(authorId);
        if (foundedAuthor is null)
        {
            var apiException = new ApiException($"Автор с Id = {authorId} не найден");
            return new Result<BookEntityModel?>(apiException);
        }

        foundedBook.Author = foundedAuthor;

        try
        {
            var updatedBook = await _bookRepository.UpdateAsync(foundedBook);
            return updatedBook;
        }
        catch (DbSavingException)
        {
            var apiException = new ApiException("Внутренняя ошибка сервера");
            return new Result<BookEntityModel?>(apiException);
        }
    }

    /// <summary>
    /// Удаляет книгу по её Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>True - если удачно, false - если книга не была найдена.</returns>
    /// <exception cref="ApiException">Возникает в случае если произошла ошибка сохранения данных.</exception>
    public async Task<Result<bool>> DeleteBookByIdAsync(int bookId)
    {
        try
        {
            return await _bookRepository.DeleteByIdAsync(bookId);
        }
        catch (DbSavingException)
        {
            // TODO: Add logs
            var apiException = new ApiException("Внутренняя ошибка сервера");
            return new Result<bool>(apiException);
        }
    }

    /// <summary>
    /// Метод получает информацию о книге по Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель книги, или null если она не была найдена.</returns>
    public async Task<BookEntityModel?> GetBookByIdAsync(int bookId)
    {
        return await _bookRepository.GetByIdAsync(bookId);
    }
}