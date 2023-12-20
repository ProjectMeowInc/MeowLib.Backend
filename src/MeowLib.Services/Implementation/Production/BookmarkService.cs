using MeowLib.DAL;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.Bookmark;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class BookmarkService(ApplicationDbContext dbContext, IChapterService chapterService, IUserService userService)
    : IBookmarkService
{
    /// <summary>
    /// Метод создаёт новую (если её нет) или обновляет старую закладку книги пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Созданную/обновлённую закладку в виде <see cref="BookmarkDto" />.</returns>
    /// <exception cref="ChapterNotFoundException">Возникает в случае, если не была найдена глава.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если не был найден пользователь.</exception>
    /// <exception cref="InnerException">Возникает в случае внутренних проблем.</exception>
    public async Task<Result<BookmarkDto>> CreateBookmarkAsync(int userId, int chapterId)
    {
        var foundedChapter = await chapterService.GetChapterByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            return Result<BookmarkDto>.Fail(new ChapterNotFoundException(chapterId));
        }

        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            return Result<BookmarkDto>.Fail(new UserNotFoundException(userId));
        }

        var alreadyExistBookmark = await GetBookmarkByUserAndChapter(foundedUser, foundedChapter);
        if (alreadyExistBookmark is not null)
        {
            var updateBookmarkResult = await UpdateBookmarkAsync(alreadyExistBookmark, foundedChapter);

            if (updateBookmarkResult.IsFailure)
            {
                return Result<BookmarkDto>.Fail(updateBookmarkResult.GetError());
            }

            var updatedBookmark = updateBookmarkResult.GetResult();
            return new BookmarkDto
            {
                Id = updatedBookmark.Id,
                ChapterId = updatedBookmark.Chapter.Id
            };
        }

        var createBookmarkResult = await CreateNewBookmarkAsync(foundedUser, foundedChapter);
        if (createBookmarkResult.IsFailure)
        {
            return Result<BookmarkDto>.Fail(createBookmarkResult.GetError());
        }

        var createdBookmark = createBookmarkResult.GetResult();
        return new BookmarkDto
        {
            Id = createdBookmark.Id,
            ChapterId = createdBookmark.Chapter.Id
        };
    }

    public Task<BookmarkDto?> GetBookmarkByUserAndBook(int userId, int bookId)
    {
        return dbContext.Bookmarks
            .Where(bookmark => bookmark.User.Id == userId && bookmark.Chapter.Translation.Book.Id == bookId)
            .Select(bookmark => new BookmarkDto
            {
                Id = bookmark.Id,
                ChapterId = bookmark.Chapter.Id
            })
            .FirstOrDefaultAsync();
    }

    private Task<BookmarkEntityModel?> GetBookmarkByUserAndChapter(UserEntityModel user,
        ChapterEntityModel chapter)
    {
        return dbContext.Bookmarks
            .FirstOrDefaultAsync(b => b.User.Id == user.Id
                                      && b.Chapter.Id == chapter.Id);
    }

    private async Task<Result<BookmarkEntityModel>> CreateNewBookmarkAsync(UserEntityModel user,
        ChapterEntityModel chapter)
    {
        var entry = await dbContext.Bookmarks.AddAsync(new BookmarkEntityModel
        {
            User = user,
            Chapter = chapter
        });
        await dbContext.SaveChangesAsync();

        return entry.Entity;
    }

    private async Task<Result<BookmarkEntityModel>> UpdateBookmarkAsync(BookmarkEntityModel bookmark,
        ChapterEntityModel newChapter)
    {
        bookmark.Chapter = newChapter;
        var entry = dbContext.Bookmarks.Update(bookmark);
        await dbContext.SaveChangesAsync();

        return entry.Entity;
    }
}