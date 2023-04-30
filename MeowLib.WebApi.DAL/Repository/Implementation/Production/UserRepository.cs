using AutoMapper;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с пользователями.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;
    
    public UserRepository(ApplicationDbContext applicationDbContext, IMapper mapper)
    {
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Создаёт пользователя в БД и возвращает его Dto-модель.
    /// </summary>
    /// <param name="createUserData">Данные для создания пользователя.</param>
    /// <returns>Dto-модель пользователя.</returns>
    public async Task<UserDto> CreateAsync(CreateUserEntityModel createUserData)
    {
        var newUser = _mapper.Map<CreateUserEntityModel, UserEntityModel>(createUserData);
        var dbResult = await _applicationDbContext.Users.AddAsync(newUser);
        
        var userDto = _mapper.Map<UserEntityModel, UserDto>(dbResult.Entity);
        
        await _applicationDbContext.SaveChangesAsync();
        return userDto;
    }

    /// <summary>
    /// Метод возвращает DTO-модель пользователя по его Id.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <returns>Dto-модель пользователя в случае успешного поиска или null если пользователь не найден</returns>
    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var foundedUser = await GetUserByIdAsync(id);
        if (foundedUser is null)
        {
            return null;
        }
        
        var userDto = _mapper.Map<UserEntityModel, UserDto>(foundedUser);

        return userDto;
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
    /// <exception cref="EntityNotFoundException">
    /// Возникает в том случае, если пользователь с заданным Id не найден
    /// </exception>
    public async Task<UserDto> UpdateAsync(int id, UpdateUserEntityModel updateUserData)
    {
        var foundedUser = await GetUserByIdAsync(id);
        if (foundedUser is null)
        {
            throw new EntityNotFoundException(nameof(UserEntityModel), "id");
        }

        foundedUser.Login = updateUserData.Login ?? foundedUser.Login;
        foundedUser.Password = updateUserData.Password ?? foundedUser.Password;

        _applicationDbContext.Users.Update(foundedUser);
        await _applicationDbContext.SaveChangesAsync();

        var userDto = _mapper.Map<UserEntityModel, UserDto>(foundedUser);
        return userDto;
    }

    /// <summary>
    /// Метод возвоаеь пользователя по его Id.
    /// </summary>
    /// <param name="id">Id для поиска.</param>
    /// <returns></returns>
    private async Task<UserEntityModel?> GetUserByIdAsync(int id)
    {
        var foundedUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        return foundedUser;
    }
}