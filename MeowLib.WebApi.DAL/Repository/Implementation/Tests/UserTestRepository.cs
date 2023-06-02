using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.WebApi.DAL.Repository.Interfaces;

namespace MeowLib.WebApi.DAL.Repository.Implementation.Tests;

public class UserTestRepository : IUserRepository
{
    private readonly List<UserEntityModel> _userData = new()
    {
        new UserEntityModel
        {
            Id = 1,
            Login = "tester",
            Role = UserRolesEnum.Admin,
            Password = "test",
        },
        new UserEntityModel
        {
            Id = 2,
            Login = "sueta",
            Role = UserRolesEnum.User,
            Password = "test"
        }
    };

    public Task<UserDto> CreateAsync(CreateUserEntityModel createUserData)
    {
        var userModel = new UserEntityModel
        {
            Id = _userData.Count,
            Login = createUserData.Login,
            Password = createUserData.Password,
            Role = UserRolesEnum.User
        };
        
        _userData.Add(userModel);
        return Task.FromResult(new UserDto
        {
            Id = userModel.Id,
            Login = userModel.Login,
            Role = userModel.Role
        });
    }

    public Task<UserEntityModel?> GetByIdAsync(int id)
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

    public Task<Result<UserDto>> UpdateAsync(int id, UpdateUserEntityModel updateUserData)
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

        return Task.FromResult(new Result<UserDto>(new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        }));
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

    public IQueryable<UserEntityModel> GetAll()
    {
        throw new NotImplementedException();
    }
}