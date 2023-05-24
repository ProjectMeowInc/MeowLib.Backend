using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Tests;

public class UserTestRepository : IUserRepository
{
    private readonly List<UserDto> _userData = new()
    {
        new UserDto
        {
            Id = 1,
            Login = "tester",
            Role = UserRolesEnum.Admin
        },
        new UserDto
        {
            Id = 2,
            Login = "sueta",
            Role = UserRolesEnum.User
        }
    };

    public Task<UserDto> CreateAsync(CreateUserEntityModel createUserData)
    {
        var userDto = new UserDto
        {
            Id = _userData.Count,
            Login = createUserData.Login,
            Role = UserRolesEnum.User
        };
        
        _userData.Add(userDto);
        return Task.FromResult(userDto);
    }

    public Task<UserDto?> GetByIdAsync(int id)
    {
        return Task.FromResult(_userData.FirstOrDefault(u => u.Id == id));
    }

    public Task<bool> DeleteByIdAsync(int id)
    {
        var foundedUser = _userData.FirstOrDefault(u => u.Id == id);
        if (foundedUser is null)
        {
            return Task.FromResult(false);
        }

        _userData.Remove(foundedUser);
        return Task.FromResult(true);
    }

    public Task<UserDto> UpdateAsync(int id, UpdateUserEntityModel updateUserData)
    {
        var foundedUser = _userData.FirstOrDefault(u => u.Id == id);
        if (foundedUser is null)
        {
            throw new EntityNotFoundException(nameof(UserEntityModel), $"Id = {id}");
        }

        if (updateUserData.Login is not null)
        {
            foundedUser.Login = updateUserData.Login;
        }

        return Task.FromResult(foundedUser);
    }

    public Task<bool> CheckForUserExistAsync(string login)
    {
        var foundedUser = _userData.FirstOrDefault(u => u.Login == login);
        return Task.FromResult(foundedUser is not null);
    }

    public Task<UserDto?> GetByLoginAndPasswordAsync(string login, string password)
    {
        throw new NotImplementedException();
    }
}