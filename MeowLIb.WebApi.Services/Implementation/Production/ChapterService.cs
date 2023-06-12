using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

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
    
    
}