using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Dto.Chapter;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с главами.
/// </summary>
public class ChapterService : IChapterService
{
    private readonly IChapterRepository _chapterRepository;
    private readonly IBookService _bookService;
    
    public ChapterService(IChapterRepository chapterRepository, IBookService bookService)
    {
        _chapterRepository = chapterRepository;
        _bookService = bookService;
    }
    
    /// <summary>
    /// Метод создаёт новую главу.
    /// </summary>
    /// <param name="name">Название главы.</param>
    /// <param name="text">Текст главы.</param>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель созданной главы.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки БД.</exception>
    public async Task<Result<ChapterEntityModel>> CreateChapterAsync(string name, string text, int bookId)
    {
        var validationErrors = new List<ValidationErrorModel>();
        
        if (name.Length < 1 || name.Length > 50)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = "Name",
                Message = "Имя не может быть меньше 1 или больше 50 символов"
            });
        }

        if (text.Length > 10000)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = "Text",
                Message = "Текст главы не может быть больше 10000 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return new Result<ChapterEntityModel>(validationException);
        }
        
        var foundedBook = await _bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(BookEntityModel), $"Id={bookId}");
            return new Result<ChapterEntityModel>(entityNotFoundException);
        }

        var newChapter = new ChapterEntityModel
        {
            Name = name,
            Text = text,
            ReleaseDate = DateTime.UtcNow,
            Book = foundedBook
        };

        var createChapterResult = await _chapterRepository.CreateAsync(newChapter);
        return createChapterResult.Match<Result<ChapterEntityModel>>(createdChapter => createdChapter, exception =>
        {
            if (exception is DbSavingException dbSavingException)
            {
                return new Result<ChapterEntityModel>(dbSavingException);
            }

            // TODO: Add logs
            return new Result<ChapterEntityModel>(exception); 
        });
    }

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Новый текст главы.</param>
    /// <returns>Модель обновлённой главы.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<ChapterEntityModel>> UpdateChapterTextAsync(int chapterId, string newText)
    {
        var updateResult = await _chapterRepository.UpdateTextAsync(chapterId, newText);

        return updateResult.Match(updatedChapter => updatedChapter, exception => new Result<ChapterEntityModel>(exception));
    }

    /// <summary>
    /// Метод возвращает главы книги в виде <see cref="ChapterDto"/>
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Модель главы в виде <see cref="ChapterDto"/></returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга с заданым Id не была найдена.</exception>
    public async Task<Result<IEnumerable<ChapterDto>>> GetAllBookChapters(int bookId)
    {
        var foundedBook = await _bookService.GetBookByIdAsync(bookId);

        if (foundedBook is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(BookEntityModel), $"Id={bookId}");
            return new Result<IEnumerable<ChapterDto>>(entityNotFoundException);
        }

        var bookChapters = await _chapterRepository.GetAll()
            .Where(chapter => chapter.Book == foundedBook)
            .OrderBy(chapter => chapter.ReleaseDate)
            .Select(chapter => new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                ReleaseDate = chapter.ReleaseDate
            }).ToListAsync();

        return bookChapters;
    }

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Option<Exception>> DeleteChapterAsync(int chapterId)
    {
        return await _chapterRepository.DeleteByIdAsync(chapterId);
    }

    /// <summary>
    /// Метод возвращает главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Модель главы, если была найдена.</returns>
    public async Task<ChapterEntityModel?> GetChapterByIdAsync(int chapterId)
    {
        return await _chapterRepository.GetByIdAsync(chapterId);
    }
}