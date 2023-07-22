using LanguageExt.Common;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с книгами.
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<BookService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="bookRepository">Репозиторий книг.</param>
    /// <param name="authorRepository">Репозиторий авторов.</param>
    /// <param name="tagRepository">Репозиторий тегов.</param>
    /// <param name="fileService">Сервис для работы с файлами.</param>
    /// <param name="logger">Логгер.</param>
    public BookService(IBookRepository bookRepository, IAuthorRepository authorRepository, ITagRepository tagRepository, 
        IFileService fileService, ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _tagRepository = tagRepository;
        _fileService = fileService;
        _logger = logger;
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
            _logger.LogError("[{@DateTime}] Ошибка сохранения книги в БД", DateTime.UtcNow);
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
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookInfoByIdAsync(int bookId, UpdateBookEntityModel updateBookEntityModel)
    {
        var inputName = updateBookEntityModel.Name?.Trim() ?? null;
        var inputDescription = updateBookEntityModel.Description?.Trim() ?? null;

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
        
        var updatedBookResult = await _bookRepository.UpdateAsync(foundedBook);
        return updatedBookResult.Match<Result<BookEntityModel?>>(updatedBook => updatedBook,
            exception => new Result<BookEntityModel?>(exception));
    }
    
    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="bookId">Id книги для обновления.</param>
    /// <param name="authorId">Id автора для обновления.</param>
    /// <returns>Обновлённую модель книги при удачном обновления, null - если книга не была найдена.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если автор не был найден.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookAuthorAsync(int bookId, int authorId)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            _logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return null;
        }

        var foundedAuthor = await _authorRepository.GetByIdAsync(authorId);
        if (foundedAuthor is null)
        {
            _logger.LogInformation("[{@DateTime}] Автор не найден", DateTime.UtcNow);
            var apiException = new EntityNotFoundException(nameof(AuthorEntityModel), $"Id={authorId}");
            return new Result<BookEntityModel?>(apiException);
        }

        foundedBook.Author = foundedAuthor;

        var updateBookResult = await _bookRepository.UpdateAsync(foundedBook);
        return updateBookResult.Match<Result<BookEntityModel?>>(updatedBook => updatedBook, 
            exception => new Result<BookEntityModel?>(exception));
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
            _logger.LogError("[{@DateTime}] Ошибка удаления книги в базе данных", DateTime.UtcNow);
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

    /// <summary>
    /// Метод обновляет список тегов книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="tags">Список Id тегов.</param>
    /// <returns>Модель книги или null, если она не была найдена.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookTagsAsync(int bookId, IEnumerable<int> tags)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            _logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return null;
        }

        var foundedTags = await _tagRepository
            .GetAll()
            .Where(tag => tags.Any(t => t == tag.Id))
            .ToListAsync();
        

        foundedBook.Tags = foundedTags;
        var updateResult = await _bookRepository.UpdateAsync(foundedBook);
        return updateResult.Match<Result<BookEntityModel?>>(updatedBook => updatedBook,
            exception => new Result<BookEntityModel?>(exception));
    }

    /// <summary>
    /// Метод обновляет обложку книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="file">Картинка для обновления.</param>
    /// <returns>Обновлённую модель книги, или null, если книга не была найдена.</returns>
    /// <exception cref="UploadingFileException">Возникает в случае ошибки загрузки файла.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookImageAsync(int bookId, IFormFile file)
    {
        // TODO: REFACTORING!!
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            _logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return null;
        }

        var uploadBookImageResult = await _fileService.UploadBookImageAsync(file);
        if (uploadBookImageResult.IsFaulted)
        {
            _logger.LogError("[{@DateTime}] Ошибка загрузки файла", DateTime.UtcNow);
            var uploadingFileException = new UploadingFileException();
            return new Result<BookEntityModel?>(uploadingFileException);
        }

        var updateBookResult = await uploadBookImageResult.MapAsync<Result<BookEntityModel>>(async fileName=>
        {
            foundedBook.ImageUrl = fileName;
            var updateBookResult = await _bookRepository.UpdateAsync(foundedBook);

            return updateBookResult.Match<Result<BookEntityModel>>(updatedBook => updatedBook, 
                exception => new Result<BookEntityModel>(exception));
        });

        return updateBookResult.Match<Result<BookEntityModel?>>(ok =>
        {
            return ok.Match<BookEntityModel?>(updatedBook => updatedBook, _ => null);
        }, exception => new Result<BookEntityModel?>(exception));
    }
}