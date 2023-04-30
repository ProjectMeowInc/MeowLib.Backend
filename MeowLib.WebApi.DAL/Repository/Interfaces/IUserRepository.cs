
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

public interface IUserRepository
{
    Task<UserDto> CreateAsync(CreateUserEntityModel createUserData);
    Task<UserDto?> GetByIdAsync(int id);
    Task<bool> DeleteByIdAsync(int id);
    Task<UserDto> UpdateAsync(int id, UpdateUserEntityModel updateUserData);
}