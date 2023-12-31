using MeowLib.DAL;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.User.Dto;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.User.Enums;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

public class UserService(ApplicationDbContext dbContext, IHashService hashService, IUserTokenService userTokenService)
    : IUserService
{
    public async Task<Result<UserEntityModel>> SignInAsync(string login, string password)
    {
        var existedUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

        if (existedUser is not null)
        {
            var apiException = new ApiException($"Пользователь с логином {login} уже существует");
            return Result<UserEntityModel>.Fail(apiException);
        }

        var validationErrors = new List<ValidationErrorModel>();

        if (login.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(login),
                Message = "Логин должен быть больше 6 символов"
            });
        }

        password = password.Trim();

        if (password.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(password),
                Message = "Пароль должен быть больше 6 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return Result<UserEntityModel>.Fail(validationException);
        }

        var hashedPassword = hashService.HashString(password);

        var createdUserEntry = await dbContext.Users.AddAsync(new UserEntityModel
        {
            Id = 0,
            Login = login,
            Password = hashedPassword,
            RefreshToken = null,
            Coins = 0,
            Role = UserRolesEnum.User
        });
        await dbContext.SaveChangesAsync();

        return createdUserEntry.Entity;
    }

    public async Task<Result<(string accessToken, string refreshToken)>> LogIn(string login, string password,
        bool isLongSession)
    {
        var hashedPassword = hashService.HashString(password);

        var foundedUser =
            await dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == hashedPassword);
        if (foundedUser is null)
        {
            var incorrectCreditionalException = new IncorrectCreditionalException("Неверный логин или пароль");
            return Result<(string accessToken, string refreshToken)>.Fail(incorrectCreditionalException);
        }

        var tokenExpiredTime = isLongSession ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);

        var accessToken = userTokenService.GenerateAccessToken(new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        });
        var refreshToken = userTokenService.GenerateRefreshToken(new RefreshTokenDataModel
        {
            Login = foundedUser.Login,
            IsLongSession = isLongSession
        }, tokenExpiredTime);

        // Если каким-то образом сгенерированный токен уже занят, то обработаем это
        var userWithSameToken = await dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (userWithSameToken is not null)
        {
            var createTokenException = new CreateTokenException("Токен уже занят");
            return Result<(string accessToken, string refreshToken)>.Fail(createTokenException);
        }

        foundedUser.RefreshToken = refreshToken;
        dbContext.Users.Update(foundedUser);
        await dbContext.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await dbContext.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Login = u.Login,
                Role = u.Role
            }).ToListAsync();
    }

    public async Task<Result<UserDto?>> UpdateUser(int id, string? login, string? password)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (login?.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(login),
                Message = "Логин не может быть меньше 6 символов"
            });
        }

        if (password?.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(password),
                Message = "Пароль не может быть меньше 6 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return Result<UserDto?>.Fail(validationException);
        }

        if (login is not null)
        {
            var loginAlreadyTaken = await CheckLoginAlreadyTaken(login);
            if (loginAlreadyTaken)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(login),
                    Message = "Такой логин уже занят"
                });

                var validationException = new ValidationException(validationErrors);
                return Result<UserDto?>.Fail(validationException);
            }
        }

        if (password is not null)
        {
            password = hashService.HashString(password);
        }

        var foundedUser = await GetUserByIdAsync(id);
        if (foundedUser is null)
        {
            return Result<UserDto?>.Ok(null);
        }

        foundedUser.Login = login ?? foundedUser.Login;
        foundedUser.Password = password ?? foundedUser.Password;

        dbContext.Users.Update(foundedUser);
        await dbContext.SaveChangesAsync();

        return new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        };
    }

    /// <summary>
    /// Метод авторизует пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Пару JWT-токенов.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если был введён некорректный токен обновления.</exception>
    public async Task<Result<(string accessToken, string refreshToken)>> LogInByRefreshTokenAsync(string refreshToken)
    {
        var parsedRefreshToken = await userTokenService.ParseRefreshTokenAsync(refreshToken);
        if (parsedRefreshToken is null)
        {
            var incorrectCreditionalException = new IncorrectCreditionalException("Неверный RefreshToken");
            return Result<(string accessToken, string refreshToken)>.Fail(incorrectCreditionalException);
        }

        var foundedUser = await GetUserByRefreshTokenAsync(refreshToken);
        if (foundedUser is null)
        {
            var sessionExpiredException = new IncorrectCreditionalException("Сессия с введёным токеном истекла");
            return Result<(string accessToken, string refreshToken)>.Fail(sessionExpiredException);
        }

        var tokenExpiredDate = parsedRefreshToken.IsLongSession
            ? DateTime.UtcNow.AddDays(30)
            : DateTime.UtcNow.AddMinutes(60);

        var newRefreshToken = userTokenService.GenerateRefreshToken(new RefreshTokenDataModel
        {
            Login = foundedUser.Login,
            IsLongSession = parsedRefreshToken.IsLongSession
        }, tokenExpiredDate);

        var newAccessToken = userTokenService.GenerateAccessToken(new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        });

        foundedUser.RefreshToken = newRefreshToken;
        dbContext.Users.Update(foundedUser);
        await dbContext.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

    public Task<UserEntityModel?> GetUserByIdAsync(int userId)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    private Task<bool> CheckLoginAlreadyTaken(string login)
    {
        return dbContext.Users.AnyAsync(u => u.Login == login);
    }

    private Task<UserEntityModel?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}