using MeowLib.Domain.Book.Dto;
using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Result;
using Microsoft.AspNetCore.Http;

namespace MeowLib.Domain.Book.Services;

public interface IBookService
{
    /// <summary>
    /// Метод создаёт новую книгу.
    /// </summary>
    /// <param name="createBookEntityModel">Данные для создания книги.</param>
    /// <returns>Модель созданной книги.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<BookEntityModel>> CreateBookAsync(BookEntityModel createBookEntityModel);

    /// <summary>
    /// Метод обновляет основную информацию о книге по её Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="name">Новое название.</param>
    /// <param name="description">Новое описание.</param>
    /// <returns>Обновлённая модель книги или null если книга не найдена.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации.</exception>
    Task<Result<BookEntityModel?>> UpdateBookInfoByIdAsync(int bookId, string? name, string? description);

    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="bookId">Id книги для обновления.</param>
    /// <param name="authorId">Id автора для обновления.</param>
    /// <returns>Обновлённую модель книги при удачном обновления, null - если книга не была найдена.</returns>
    Task<Result<BookEntityModel?>> UpdateBookAuthorAsync(int bookId, int authorId);

    /// <summary>
    /// Удаляет книгу по её Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>True - если удачно, false - если книга не была найдена.</returns>
    /// <exception cref="ApiException">Возникает в случае если произошла ошибка сохранения данных.</exception>
    Task<Result<bool>> DeleteBookByIdAsync(int bookId);

    /// <summary>
    /// Метод получает информацию о книге по Id.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель книги, или null если она не была найдена.</returns>
    Task<BookEntityModel?> GetBookByIdAsync(int bookId);

    /// <summary>
    /// Метод обновляет список тегов книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="tags">Список Id тегов.</param>
    /// <returns>Модель книги или null, если она не была найдена.</returns>
    Task<Result<BookEntityModel?>> UpdateBookTagsAsync(int bookId, IEnumerable<int> tags);

    /// <summary>
    /// Метод обновляет обложку книги.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="file">Картинка для обновления.</param>
    /// <returns>Обновлённую модель книги, или null, если книга не была найдена.</returns>
    /// <exception cref="UploadingFileException">Возникает в случае ошибки загрузки файла.</exception>
    Task<Result<BookEntityModel?>> UpdateBookImageAsync(int bookId, IFormFile file);

    /// <summary>
    /// Метод получает все книги.
    /// </summary>
    /// <returns>Все книги в виде Dto.</returns>
    Task<List<BookDto>> GetAllBooksAsync();
}