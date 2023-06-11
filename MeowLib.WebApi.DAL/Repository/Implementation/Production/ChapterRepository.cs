using LanguageExt;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

public class ChapterRepository : IChapterRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ChapterRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

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
            return new Result<ChapterEntityModel>(dbSavingException);
        }

        return createdChapter.Entity;
    }

    public async Task<ChapterEntityModel?> GetByIdAsync(int chapterId)
    {
        return await _applicationDbContext.Chapters.FirstOrDefaultAsync(c => c.Id == chapterId);
    }

    public async Task<Option<Exception>> DeleteAsync(ChapterEntityModel chapter)
    {
        try
        {
            _applicationDbContext.Chapters.Remove(chapter);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(ChapterEntityModel), DbSavingTypesEnum.Delete);
            return Option<Exception>.Some(dbSavingException);
        }
        
        return Option<Exception>.None;
    }

    public async Task<Option<Exception>> DeleteByIdAsync(int chapterId)
    {
        var foundedChapter = await GetByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(ChapterEntityModel), $"Id={chapterId}");
            return Option<Exception>.Some(entityNotFoundException);
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
            return new Result<ChapterEntityModel>(dbSavingException);
        }
    }

    public Result<ChapterEntityModel> UpdateBookAsync(int chapterId, BookEntityModel book)
    {
        throw new NotImplementedException();
    }

    public Result<ChapterEntityModel> UpdateTextAsync(int chapterId, string newText)
    {
        throw new NotImplementedException();
    }
}