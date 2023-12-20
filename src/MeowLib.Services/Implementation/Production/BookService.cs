using MeowLib.DAL;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.TranslationEntity;
using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Author;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с книгами.
/// </summary>
public class BookService(
    IFileService fileService,
    ILogger<BookService> logger,
    ApplicationDbContext dbContext,
    IAuthorService authorService)
    : IBookService
{
    public async Task<Result<BookEntityModel>> CreateBookAsync(BookEntityModel createBookEntityModel)
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
            return Result<BookEntityModel>.Fail(new ValidationException(nameof(BookService), validationErrors));
        }

        // todo: fix author
        var entry = await dbContext.Books.AddAsync(new BookEntityModel
        {
            Name = inputName,
            Description = inputDescription,
            Author = null,
            Tags = new List<TagEntityModel>(),
            Translations = new List<TranslationEntityModel>()
        });
        return entry.Entity;
    }

    public async Task<Result<BookEntityModel?>> UpdateBookInfoByIdAsync(int bookId, string? name, string? description)
    {
        var inputName = name?.Trim();
        var inputDescription = description?.Trim();

        var validationErrors = new List<ValidationErrorModel>();

        if (inputName is not null)
        {
            if (inputName.Length < 5)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(name),
                    Message = "Имя книги не может быть меньше 5 символов"
                });
            }
        }
        
        if (validationErrors.Any())
        {
            var validationException = new ValidationException(nameof(BookService), validationErrors);
            return Result<BookEntityModel?>.Fail(validationException);
        }

        var foundedBook = await GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<BookEntityModel?>.Ok(null);
        }

        foundedBook.Name = inputName ?? foundedBook.Name;
        foundedBook.Description = inputDescription ?? foundedBook.Description;

        dbContext.Books.Update(foundedBook);
        await dbContext.SaveChangesAsync();

        return Result<BookEntityModel?>.Ok(foundedBook);
    }

    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="bookId">Id книги для обновления.</param>
    /// <param name="authorId">Id автора для обновления.</param>
    /// <returns>Обновлённую модель книги при удачном обновления, null - если книга не была найдена.</returns>
    public async Task<Result<BookEntityModel?>> UpdateBookAuthorAsync(int bookId, int authorId)
    {
        var foundedBook = await GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return Result<BookEntityModel?>.Ok(null);
        }

        var foundedAuthor = await authorService.GetAuthorByIdAsync(authorId);
        if (foundedAuthor is null)
        {
            logger.LogInformation("[{@DateTime}] Автор не найден", DateTime.UtcNow);
            return Result<BookEntityModel?>.Fail(new AuthorNotFoundException(authorId));
        }

        foundedBook.Author = foundedAuthor;

        dbContext.Books.Update(foundedBook);
        await dbContext.SaveChangesAsync();

        return foundedBook;
    }

    /// <summary>
    /// Удаляет книгу по её Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>True - если удачно, false - если книга не была найдена.</returns>
    /// <exception cref="ApiException">Возникает в случае если произошла ошибка сохранения данных.</exception>
    public async Task<Result<bool>> DeleteBookByIdAsync(int bookId)
    {
        var foundedBook = await GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return false;
        }

        dbContext.Books.Remove(foundedBook);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает информацию о книге по Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель книги, или null если она не была найдена.</returns>
    public Task<BookEntityModel?> GetBookByIdAsync(int bookId)
    {
        return dbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
    }

    /// <summary>
    /// Метод обновляет список тегов книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="tags">Список Id тегов.</param>
    /// <returns>Модель книги или null, если она не была найдена.</returns>
    public async Task<Result<BookEntityModel?>> UpdateBookTagsAsync(int bookId, IEnumerable<int> tags)
    {
        var foundedBook = await GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return Result<BookEntityModel?>.Ok(null);
        }

        var foundedTags = await dbContext.Tags
            .Where(tag => tags.Any(t => t == tag.Id))
            .ToListAsync();

        foundedBook.Tags = foundedTags;

        dbContext.Books.Update(foundedBook);
        await dbContext.SaveChangesAsync();

        return foundedBook;
    }

    /// <summary>
    /// Метод обновляет обложку книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="file">Картинка для обновления.</param>
    /// <returns>Обновлённую модель книги, или null, если книга не была найдена.</returns>
    /// <exception cref="UploadingFileException">Возникает в случае ошибки загрузки файла.</exception>
    public async Task<Result<BookEntityModel?>> UpdateBookImageAsync(int bookId, IFormFile file)
    {
        // TODO: REFACTORING!!
        var foundedBook = await GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            logger.LogInformation("[{@DateTime}] Книга не найдена", DateTime.UtcNow);
            return Result<BookEntityModel?>.Ok(null);
        }

        var uploadBookImageResult = await fileService.UploadBookImageAsync(file);
        if (uploadBookImageResult.IsFailure)
        {
            logger.LogError("[{@DateTime}] Ошибка загрузки файла", DateTime.UtcNow);
            var uploadingFileException = new UploadingFileException();
            return Result<BookEntityModel?>.Fail(uploadingFileException);
        }

        var uploadedFile = uploadBookImageResult.GetResult();
        foundedBook.ImageUrl = uploadedFile;

        dbContext.Books.Update(foundedBook);
        await dbContext.SaveChangesAsync();

        return foundedBook;
    }

    public Task<List<BookDto>> GetAllBooksAsync()
    {
        return dbContext.Books.Select(b => new BookDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                ImageName = b.ImageUrl
            })
            .ToListAsync();
    }
}