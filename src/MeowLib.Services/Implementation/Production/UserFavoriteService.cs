using MeowLib.DAL;
using MeowLib.Domain.Author.Dto;
using MeowLib.Domain.Book.Dto;
using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.Book.Exceptions;
using MeowLib.Domain.Book.Services;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.User.Exceptions;
using MeowLib.Domain.User.Services;
using MeowLib.Domain.UserFavorite.Dto;
using MeowLib.Domain.UserFavorite.Entity;
using MeowLib.Domain.UserFavorite.Enums;
using MeowLib.Domain.UserFavorite.Services;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class UserFavoriteService(ApplicationDbContext dbContext, IUserService userService, IBookService bookService)
    : IUserFavoriteService
{
    /// <summary>
    /// Метод обновляет книгу в списке пользователя.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="status">Статус для обновления</param>
    /// <returns>Обновлённую книгу в списке пользователя.</returns>
    public async Task<Result<UserFavoriteEntityModel>> AddOrUpdateUserListAsync(int bookId, int userId,
        UserFavoritesStatusEnum status)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            return Result<UserFavoriteEntityModel>.Fail(new BookNotFoundException(bookId));
        }

        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            return Result<UserFavoriteEntityModel>.Fail(new UserNotFoundException(userId));
        }

        var getUserFavoriteResult = await GetUserFavoriteByBookAsync(userId, bookId);
        if (getUserFavoriteResult.IsFailure)
        {
            var exception = getUserFavoriteResult.GetError();
            return Result<UserFavoriteEntityModel>.Fail(
                new InnerException($"Получения избранного пользователя по книге: {exception.Message}"));
        }

        var userFavorite = getUserFavoriteResult.GetResult();
        if (userFavorite is null)
        {
            return await AddNewAsync(foundedBook, foundedUser, status);
        }

        return await UpdateOldAsync(userFavorite, status);
    }

    /// <summary>
    /// Метод получает список избранных книг пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns>Список избранныъ книг пользователя.</returns>
    public Task<List<UserFavoriteDto>> GetUserFavoritesAsync(int userId)
    {
        return dbContext.UsersFavorite
            .Where(uf => uf.User.Id == userId)
            .Select(uf => new UserFavoriteDto
            {
                Status = uf.Status,
                Book = new BookDto
                {
                    Id = uf.Book.Id,
                    Name = uf.Book.Name,
                    Description = uf.Book.Description,
                    ImageName = uf.Book.Image != null ? uf.Book.Image.FileSystemName : null,
                    Author = uf.Book.Author != null
                        ? new PeopleDto
                        {
                            Id = uf.Book.Author.Id,
                            Name = uf.Book.Author.Name
                        }
                        : null
                }
            })
            .ToListAsync();
    }

    /// <summary>
    /// Метод получает книгу в списке пользователя по её Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="bookId">Id книги.</param>
    /// <returns>Информацию о книге, если она была найдена. Иначе - null</returns>
    /// <exception cref="BookNotFoundException">Возникает в случае, если книга не была найдена.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    public async Task<Result<UserFavoriteEntityModel?>> GetUserFavoriteByBookAsync(int userId, int bookId)
    {
        var foundedBook = await bookService.GetBookByIdAsync(bookId);
        if (foundedBook is null)
        {
            var bookNotFoundException = new BookNotFoundException(bookId);
            return Result<UserFavoriteEntityModel?>.Fail(bookNotFoundException);
        }

        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            var userNotFoundException = new UserNotFoundException(userId);
            return Result<UserFavoriteEntityModel?>.Fail(userNotFoundException);
        }

        var foundedUserFavorite =
            await dbContext.UsersFavorite.FirstOrDefaultAsync(uf => uf.User.Id == userId && uf.Book.Id == bookId);

        return foundedUserFavorite;
    }

    private async Task<Result<UserFavoriteEntityModel>> AddNewAsync(BookEntityModel book, UserEntityModel user,
        UserFavoritesStatusEnum status)
    {
        var entry = await dbContext.UsersFavorite.AddAsync(new UserFavoriteEntityModel
        {
            Book = book,
            User = user,
            Status = status
        });
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    private async Task<Result<UserFavoriteEntityModel>> UpdateOldAsync(UserFavoriteEntityModel userFavoriteEntity,
        UserFavoritesStatusEnum status)
    {
        userFavoriteEntity.Status = status;
        dbContext.UsersFavorite.Update(userFavoriteEntity);
        await dbContext.SaveChangesAsync();
        return userFavoriteEntity;
    }
}