using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервси для работы с пользователями.
/// </summary>
public class UserService : IUserService
{
    private readonly IHashService _hashService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    
    public UserService(IHashService hashService, IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _hashService = hashService;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Метод создаёт нового пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="password">Пароль пользователя</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="ApiException">Возникает в случае если логин пользователя занят.</exception>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    public async Task<Result<UserDto>> SignInAsync(string login, string password)
    {
        var loginAlreadyExist = await _userRepository.CheckForUserExistAsync(login);

        if (loginAlreadyExist)
        {
            var apiException = new ApiException($"Пользователь с логином {login} уже существует");
            return new Result<UserDto>(apiException);
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
            return new Result<UserDto>(validationException);
        }
        
        var hashedPassword = _hashService.HashString(password);
        var userData = new CreateUserEntityModel
        {
            Login = login,
            Password = hashedPassword
        };

        var createdUser = await _userRepository.CreateAsync(userData);
        return createdUser;
    }

    /// <summary>
    /// Метод создаёт JWT-токег для авторизации пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>JWT-токен для авторизации.</returns>
    /// <exception cref="ApiException">Возникает в случае если указан неверный логин или пароль</exception>
    public async Task<Result<string>> LogIn(string login, string password)
    {
        var hashedPassword = _hashService.HashString(password);

        var userData = await _userRepository.GetByLoginAndPasswordAsync(login, hashedPassword);
        if (userData is null)
        {
            var apiException = new ApiException("Неверный логин или пароль");
            return new Result<string>(apiException);
        }

        var userToken = _jwtTokenService.GenerateToken(userData);
        return userToken;
    }
}