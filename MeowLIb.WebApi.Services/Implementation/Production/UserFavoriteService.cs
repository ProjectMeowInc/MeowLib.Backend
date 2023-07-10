using LanguageExt.Common;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Dto.UserFavorite;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class UserFavoriteService : IUserFavoriteService
{
    private readonly IUserFavoriteRepository _userFavoriteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IBookRepository _bookRepository;
    
    public UserFavoriteService(IUserFavoriteRepository userFavoriteRepository, IUserRepository userRepository, 
        IBookRepository bookRepository)
    {
        _userFavoriteRepository = userFavoriteRepository;
        _userRepository = userRepository;
        _bookRepository = bookRepository;
    }

    /// <summary>
    /// Метод обновляет книгу в списке пользователя.
    /// </summary>
    /// <param name="bookId">Id книги.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="status">Статус для обновления</param>
    /// <returns>Обновлённую книгу в списке пользователя.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если книга или пользователь не были найдены.</exception>
    public async Task<Result<UserFavoriteEntityModel>> AddOrUpdateUserListAsync(int bookId, int userId, 
        UserFavoritesStatusEnum status)
    {
        var foundedBook = await _bookRepository.GetByIdAsync(bookId);
        if (foundedBook is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(BookEntityModel), $"Id={bookId}");
            return new Result<UserFavoriteEntityModel>(entityNotFoundException);
        }
        
        var foundedUser = await _userRepository.GetByIdAsync(userId);
        if (foundedUser is null)
        {
            var entityNotFoundException = new EntityNotFoundException(nameof(UserEntityModel), $"Id={userId}");
            return new Result<UserFavoriteEntityModel>(entityNotFoundException);
        }

        var existedUserFavorites = await _userFavoriteRepository.GetByBookAndUserAsync(foundedBook, foundedUser);
        if (existedUserFavorites is null)
        {
            return await AddNewAsync(foundedBook, foundedUser, status);
        }

        return await UpdateOldAsync(existedUserFavorites, status);
    }

    /// <summary>
    /// Метод получает список избранных книг пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя</param>
    /// <returns>Список избранныъ книг пользователя.</returns>
    public async Task<List<UserFavoriteDto>> GetUserFavorites(int userId)
    {

        return await _userFavoriteRepository.GetAll()
            .Where(uf => uf.User.Id == userId)
            .Select(uf => new UserFavoriteDto
            {
                Status = uf.Status,
                Book = new BookDto
                {
                    Id = uf.Book.Id,
                    Name = uf.Book.Name,
                    Description = uf.Book.Description
                }
            })
            .ToListAsync();
    }

    
    private async Task<Result<UserFavoriteEntityModel>> AddNewAsync(BookEntityModel book, UserEntityModel user,
        UserFavoritesStatusEnum status)
    {
        return await _userFavoriteRepository.CreateAsync(new UserFavoriteEntityModel
        {
            Book = book,
            User = user,
            Status = status
        });
    }

    private async Task<Result<UserFavoriteEntityModel>> UpdateOldAsync(UserFavoriteEntityModel userFavoriteEntity, 
        UserFavoritesStatusEnum status)
    {
        userFavoriteEntity.Status = status;
        return await _userFavoriteRepository.UpdateAsync(userFavoriteEntity);
    }
}