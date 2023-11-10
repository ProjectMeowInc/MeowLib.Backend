using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Result;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с главами.
/// </summary>
public class ChapterRepository : IChapterRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных.</param>
    public ChapterRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод создаёт новую главу в базе данных.
    /// </summary>
    /// <param name="chapter">Модель главы для создания.</param>
    /// <returns>Модель созданной книги.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<ChapterEntityModel>> CreateAsync(ChapterEntityModel chapter)
    {
        var createdChapter = await _applicationDbContext.Chapters.AddAsync(chapter);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Create);
            return Result<ChapterEntityModel>.Fail(dbSavingException);
        }

        return createdChapter.Entity;
    }

    public async Task<ChapterEntityModel?> GetByIdAsync(int chapterId)
    {
        return await _applicationDbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
    }

    /// <summary>
    /// Метод удаляет главу.
    /// </summary>
    /// <param name="chapter">Глава для удаления.</param>
    /// <returns>Ошибку, если она есть.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result> DeleteAsync(ChapterEntityModel chapter)
    {
        try
        {
            _applicationDbContext.Chapters.Remove(chapter);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Delete);
            return Result.Fail(dbSavingException);
        }

        return Result.Ok();
    }

    /// <summary>
    /// Метод удаляет главу по её Id.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Ошибку, если она есть. Так же возращает ошибки метода <see cref="DeleteAsync"/>.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если глава не была найдена.</exception>
    public async Task<Result> DeleteByIdAsync(int chapterId)
    {
        var foundedChapter = await GetByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(ChapterEntityModel), $"Id={chapterId}");
            return Result.Fail(entityNotFoundException);
        }

        return await DeleteAsync(foundedChapter);
    }

    public async Task<Result<ChapterEntityModel>> UpdateAsync(ChapterEntityModel chapter)
    {
        try
        {
            var entryEntity = _applicationDbContext.Chapters.Entry(chapter);
            _applicationDbContext.Chapters.Update(entryEntity.Entity);
            await _applicationDbContext.SaveChangesAsync();
            return entryEntity.Entity;
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Update);
            return Result<ChapterEntityModel>.Fail(dbSavingException);
        }
    }

    public Task<Result<ChapterEntityModel>> UpdateBookAsync(int chapterId, BookEntityModel book)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Метод обновляет текст главы.
    /// </summary>
    /// <param name="chapterId">Id главы.</param>
    /// <param name="newText">Текст для обновления.</param>
    /// <returns>Модель главы.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если сущность не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<ChapterEntityModel>> UpdateTextAsync(int chapterId, string newText)
    {
        var foundedChapter = await GetByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(ChapterEntityModel), $"Id={chapterId}");
            return Result<ChapterEntityModel>.Fail(entityNotFoundException);
        }

        foundedChapter.Text = newText;

        var updateResult = _applicationDbContext.Chapters.Update(foundedChapter);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Update);
            return Result<ChapterEntityModel>.Fail(dbSavingException);
        }

        return updateResult.Entity;
    }

    public IQueryable<ChapterEntityModel> GetAll()
    {
        return _applicationDbContext.Chapters.AsQueryable();
    }
}