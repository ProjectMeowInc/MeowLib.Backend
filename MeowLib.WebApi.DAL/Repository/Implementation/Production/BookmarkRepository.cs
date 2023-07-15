using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

public class BookmarkRepository : IBookmarkRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public BookmarkRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<BookmarkEntityModel>> CreateAsync(BookmarkEntityModel entity)
    {
        var result = await _applicationDbContext.Bookmarks.AddAsync(entity);
        
        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return result.Entity;
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(BookmarkEntityModel), DbSavingTypesEnum.Create);
            return new Result<BookmarkEntityModel>(dbSavingException);
        }
    }
    
    public async Task<BookmarkEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Bookmarks
            .Include(b => b.Chapter)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
        
    public IQueryable<BookmarkEntityModel> GetAll()
    {
        return _applicationDbContext.Bookmarks.AsQueryable();
    }
        
    public async Task<Result<bool>> DeleteByIdAsync(int id)
    {
        var foundedBookmark = await GetByIdAsync(id);
        if (foundedBookmark is null)
        {
            return false;
        }

        try
        {
            _applicationDbContext.Bookmarks.Remove(foundedBookmark);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(BookmarkEntityModel), DbSavingTypesEnum.Delete);
            return new Result<bool>(dbSavingException);
        }
        
        return true;
    }

    public async Task<BookmarkEntityModel?> GetByUserAndChapterAsync(UserEntityModel user, ChapterEntityModel chapter)
    {
        return await _applicationDbContext.Bookmarks
            .Include(bookmark => bookmark.Chapter)
            .FirstOrDefaultAsync(bookMark =>
                bookMark.User == user && bookMark.Chapter.Book == chapter.Book);
    }
    
    public async Task<Result<BookmarkEntityModel>> UpdateAsync(BookmarkEntityModel entity)
    {
        try
        {
            var entryEntity = _applicationDbContext.Entry(entity);
            entryEntity.State = EntityState.Modified;
            await _applicationDbContext.SaveChangesAsync();
            return entryEntity.Entity;
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(BookmarkEntityModel), DbSavingTypesEnum.Update);
            return new Result<BookmarkEntityModel>(dbSavingException);
        }
    }
}