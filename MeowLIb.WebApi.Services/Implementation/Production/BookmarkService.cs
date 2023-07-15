using AutoMapper;
using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.Bookmark;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.User;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class BookmarkService : IBookmarkService
{
    private readonly IBookmarkRepository _bookmarkRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public BookmarkService(IBookmarkRepository bookmarkRepository, IChapterRepository chapterRepository, 
        IUserRepository userRepository, IMapper mapper)
    {
        _bookmarkRepository = bookmarkRepository;
        _chapterRepository = chapterRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод создаёт новую (если её нет) или обновляет старую закладку книги пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="chapterId">Id главы.</param>
    /// <returns>Созданную/обновлённую закладку в виде <see cref="BookmarkDto"/>.</returns>
    /// <exception cref="ChapterNotFoundException">Возникает в случае, если не была найдена глава.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если не был найден пользователь.</exception>
    /// <exception cref="InnerException">Возникает в случае внутренних проблем.</exception>
    public async Task<Result<BookmarkDto>> CreateBookmarkAsync(int userId, int chapterId)
    {
        var foundedChapter = await _chapterRepository.GetByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            var chapterNotFoundException = new ChapterNotFoundException(chapterId);
            return new Result<BookmarkDto>(chapterNotFoundException);
        }
        
        var foundedUser = await _userRepository.GetByIdAsync(userId);
        if (foundedUser is null)
        {
            var userNotFoundException = new UserNotFoundException(userId);
            return new Result<BookmarkDto>(userNotFoundException);
        }

        var alreadyExistBookmark = await GetBookmarkByUserAndChapter(foundedUser, foundedChapter);
        if (alreadyExistBookmark is not null)
        {
            var updateBookmarkResult = await UpdateBookmarkAsync(alreadyExistBookmark, foundedChapter);
            
            return updateBookmarkResult.Match<Result<BookmarkDto>>(updatedBookmark => 
                    _mapper.Map<BookmarkEntityModel, BookmarkDto>(updatedBookmark), 
                exception => new Result<BookmarkDto>(exception));
        }

        var createBookmarkResult = await CreateNewBookmarkAsync(foundedUser, foundedChapter);
        
        return createBookmarkResult.Match<Result<BookmarkDto>>(createdBookmark => 
            _mapper.Map<BookmarkEntityModel, BookmarkDto>(createdBookmark), 
            exception => new Result<BookmarkDto>(exception));
    }

    private async Task<BookmarkEntityModel?> GetBookmarkByUserAndChapter(UserEntityModel user, ChapterEntityModel chapter)
    {
       return await _bookmarkRepository.GetByUserAndChapterAsync(user, chapter);
    }

    private async Task<Result<BookmarkEntityModel>> CreateNewBookmarkAsync(UserEntityModel user, ChapterEntityModel chapter)
    {
        var createBookmarkResult = await _bookmarkRepository.CreateAsync(new BookmarkEntityModel
        {
            User = user,
            Chapter = chapter
        });

        return createBookmarkResult.Match<Result<BookmarkEntityModel>>(createdBookmark => createdBookmark, exception =>
        {
            if (exception is DbSavingException)
            {
                // TODO: ADD LOGS
            }
            
            // TODO: ADD LOGS FOR THIS
            var innerException = new InnerException(exception.Message);
            return new Result<BookmarkEntityModel>(innerException);
        });
    }
    
    private async Task<Result<BookmarkEntityModel>> UpdateBookmarkAsync(BookmarkEntityModel bookmark, ChapterEntityModel newChapter)
    {
        bookmark.Chapter = newChapter;
        var updateBookmarkResult = await _bookmarkRepository.UpdateAsync(bookmark);
        return updateBookmarkResult.Match<Result<BookmarkEntityModel>>(updatedBookmark => updatedBookmark, exception =>
        {
            if (exception is DbSavingException)
            {
                // TODO: ADD LOGS
            }
            
            // TODO: ADD LOGS FOR THIS
            var innerException = new InnerException(exception.Message);
            return new Result<BookmarkEntityModel>(innerException);
        });
    }
}