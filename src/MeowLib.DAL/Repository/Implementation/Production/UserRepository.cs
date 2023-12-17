using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с пользователями.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public UserRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Создаёт пользователя в БД и возвращает его Dto-модель.
    /// </summary>
    /// <param name="createUserData">Данные для создания пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    public async Task<UserDto> CreateAsync(CreateUserEntityModel createUserData)
    {
        var dbResult = await _applicationDbContext.Users.AddAsync(new UserEntityModel
        {
            Login = createUserData.Login,
            Password = createUserData.Password,
            RefreshToken = null,
            Coins = 0,
            Role = UserRolesEnum.User
        });
        await _applicationDbContext.SaveChangesAsync();

        var createdUser = dbResult.Entity;
        return new UserDto
        {
            Id = createdUser.Id,
            Login = createdUser.Login,
            Role = createdUser.Role
        };
    }

    /// <summary>
    /// Метод возвращает модель пользователя по его Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>Модель пользователя в случае успешного поиска или null если пользователь не найден</returns>
    public async Task<UserEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <summary>
    /// Метод удаляет пользователя по Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>True - в случае удачного удаления, иначе - false</returns>
    public async Task<bool> DeleteByIdAsync(int id)
    {
        var foundedUser = await GetUserByIdAsync(id);
        if (foundedUser is null)
        {
            return false;
        }

        try
        {
            _applicationDbContext.Users.Remove(foundedUser);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Метод обновляет информацию о пользователе.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="updateUserData">Данные для обновления.</param>
    /// <returns>Dto-модель пользователя.</returns>
    public async Task<Result<UserDto?>> UpdateAsync(int id, UpdateUserEntityModel updateUserData)
    {
        var foundedUser = await GetUserByIdAsync(id);
        if (foundedUser is null)
        {
            return Result<UserDto?>.Ok(null);
        }

        foundedUser.Login = updateUserData.Login ?? foundedUser.Login;
        foundedUser.Password = updateUserData.Password ?? foundedUser.Password;
        foundedUser.Role = updateUserData.Role ?? foundedUser.Role;

        var updatedUser = _applicationDbContext.Users.Update(foundedUser).Entity;
        await _applicationDbContext.SaveChangesAsync();

        return new UserDto
        {
            Id = updatedUser.Id,
            Login = updatedUser.Login,
            Role = updatedUser.Role
        };
    }

    /// <summary>
    /// Метод проверяет существует ли пользователь с заданным логином.
    /// </summary>
    /// <param name="login">Логин для проверки.</param>
    /// <returns>True - если существует, иначе - false</returns>
    public async Task<bool> CheckForUserExistAsync(string login)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        return user != null;
    }

    /// <summary>
    /// Метод получает пользователя по логину и паролю
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Хеш пароля пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    public async Task<UserDto?> GetByLoginAndPasswordAsync(string login, string password)
    {
        var foundedUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Login == login &&
            u.Password == password);

        if (foundedUser is null)
        {
            return null;
        }

        return new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        };
    }

    /// <summary>
    /// Метод получает список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей в формате IQueryable</returns>
    public IQueryable<UserEntityModel> GetAll()
    {
        return _applicationDbContext.Users.AsQueryable();
    }

    /// <summary>
    /// Метод получает пользователя по логину.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <returns>Модель пользователя или null если он не был найден.</returns>
    public async Task<UserEntityModel?> GetByLoginAsync(string login)
    {
        return await _applicationDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    /// <summary>
    /// Метод получает пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Найденного пользователя или null</returns>
    public async Task<UserEntityModel?> GetByRefreshTokenAsync(string refreshToken)
    {
        var foundedUser = await _applicationDbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        return foundedUser;
    }

    /// <summary>
    /// Метод изменяет токена обновления пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="newRefreshToken">Новый токен обновления.</param>
    /// <returns>Ошибку, если она есть.</returns>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    public async Task<Result> UpdateRefreshTokenAsync(string login, string newRefreshToken)
    {
        var foundedUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Login == login);
        if (foundedUser is null)
        {
            return Result.Fail(new UserNotFoundException(-1));
        }

        foundedUser.RefreshToken = newRefreshToken;
        _applicationDbContext.Users.Update(foundedUser);

        await _applicationDbContext.SaveChangesAsync();

        return Result.Ok();
    }

    /// <summary>
    /// Метод возвращает пользователя по его Id.
    /// </summary>
    /// <param name="id">Id для поиска.</param>
    /// <returns>Модель пользователя если нашёл, иначе - null</returns>
    private async Task<UserEntityModel?> GetUserByIdAsync(int id)
    {
        var foundedUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        return foundedUser;
    }
}