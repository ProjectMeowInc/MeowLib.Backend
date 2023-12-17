﻿using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL.Repository.Implementation.Production;

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

        await _applicationDbContext.SaveChangesAsync();
        return result.Entity;
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

        _applicationDbContext.Bookmarks.Remove(foundedBookmark);
        await _applicationDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<BookmarkEntityModel?> GetByUserAndChapterAsync(UserEntityModel user, ChapterEntityModel chapter)
    {
        await _applicationDbContext.Chapters
            .Entry(chapter)
            .Reference(c => c.Translation.Book).LoadAsync();

        return await _applicationDbContext.Bookmarks
            .Include(bookmark => bookmark.Chapter)
            .FirstOrDefaultAsync(bookMark =>
                bookMark.User == user && bookMark.Chapter.Translation.Book == chapter.Translation.Book);
    }

    public async Task<Result<BookmarkEntityModel>> UpdateAsync(BookmarkEntityModel entity)
    {
        var entryEntity = _applicationDbContext.Entry(entity);
        entryEntity.State = EntityState.Modified;
        await _applicationDbContext.SaveChangesAsync();
        return entryEntity.Entity;
    }
}