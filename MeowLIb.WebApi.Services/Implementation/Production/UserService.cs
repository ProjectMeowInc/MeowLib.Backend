using LanguageExt.Common;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервси для работы с пользователями.
/// </summary>
public class UserService : IUserService
{
    private readonly IHashService _hashService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hashService">Сервис для хеширования.</param>
    /// <param name="userRepository">Репозиторий пользователей.</param>
    /// <param name="jwtTokenService">Сервис JWT-токенов.</param>
    /// <param name="logger">Логгер</param>
    public UserService(IHashService hashService, IUserRepository userRepository, IJwtTokenService jwtTokenService, 
        ILogger<UserService> logger)
    {
        _hashService = hashService;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
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
    /// Метод генерирует пару JWT-токенов для авторизации пользователя.
    /// </summary>
    /// <param name="login">Логин пользователя.</param>
    /// <param name="password">Пароль пользователя.</param>
    /// <param name="isLongSession">True - RefreshToken будет создан на 30 дней, False - 30 минут.</param>
    /// <returns>Пару JWT-токенов для авторизации.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если авторизационные данные некорректны.</exception>
    /// <exception cref="CreateTokenException">Возникает в случае, если сгенерированные токен уже кому-то принадлежит.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если пользователь не был найден.</exception>
    public async Task<Result<(string accessToken, string refreshToken)>> LogIn(string login, string password, bool isLongSession)
    {
        var hashedPassword = _hashService.HashString(password);

        var userData = await _userRepository.GetByLoginAndPasswordAsync(login, hashedPassword);
        if (userData is null)
        {
            var incorrectCreditionalException = new IncorrectCreditionalException("Неверный логин или пароль");
            return new Result<(string accessToken, string refreshToken)>(incorrectCreditionalException);
        }

        var tokenExpiredTime = isLongSession ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddMinutes(30);
        
        var accessToken = _jwtTokenService.GenerateAccessToken(userData);
        var refreshToken = _jwtTokenService.GenerateRefreshToken(new RefreshTokenDataModel
        {
            Login = userData.Login,
            IsLongSession = isLongSession
        }, tokenExpiredTime);

        // Если каким-то образом сгенерированный токен уже занят, то обработаем это
        var foundedUser = await _userRepository.GetByRefreshTokenAsync(refreshToken);

        if (foundedUser is not null)
        {
            var createTokenException = new CreateTokenException("Токен уже занят");
            return new Result<(string accessToken, string refreshToken)>(createTokenException);
        }

        var updateError = await _userRepository.UpdateRefreshTokenAsync(userData.Login, refreshToken);
        return updateError.Match<Result<(string accessToken, string refreshToken)>>(exception => 
        {
            _logger.LogError("Ошибка обновления Refresh-токена: {}", exception.Message);
            
            if (exception is EntityNotFoundException entityNotFoundException)
            {
                return new Result<(string accessToken, string refreshToken)>(entityNotFoundException);
            }
            
            return new Result<(string accessToken, string refreshToken)>(exception);
        }, () => (accessToken, refreshToken));
    }

    /// <summary>
    /// Метод получает список всех пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        return await _userRepository.GetAll().Select(u => new UserDto
        {
            Id = u.Id,
            Login = u.Login,
            Role = u.Role
        }).ToListAsync();
    }
    
    /// <summary>
    /// Метод обновляет информацию о пользователе и возвращает его Dto-модель.
    /// </summary>
    /// <param name="id">Id пользователя.</param>
    /// <param name="updateData">Данные для обновления.</param>
    /// <returns>Dto-модель пользователя.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если входные данные были невалидны.</exception>
    /// <exception cref="EntityNotFoundException">Возникает в том случае, если пользователь с заданным Id не найден.</exception>
    public async Task<Result<UserDto>> UpdateUser(int id, UpdateUserEntityModel updateData)
    {
        var validationErrors = new List<ValidationErrorModel>();
        
        if (updateData.Login is not null && updateData.Login.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(updateData.Login),
                Message = "Логин не может быть меньше 6 символов"
            });
        }

        if (updateData.Password is not null && updateData.Password.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(updateData.Password),
                Message = "Пароль не может быть меньше 6 символов"
            });
        }

        if (validationErrors.Any())
        {
            var validationException = new ValidationException(validationErrors);
            return new Result<UserDto>(validationException);
        }

        if (updateData.Login is not null)
        {
            var foundedUser = await _userRepository.GetByLoginAsync(updateData.Login);
            if (foundedUser is not null)
            {
                validationErrors.Add(new ValidationErrorModel
                {
                    PropertyName = nameof(updateData.Login),
                    Message = "Такой логин уже занят"
                });

                var validationException = new ValidationException(validationErrors);
                return new Result<UserDto>(validationException);
            }
        }

        if (updateData.Password is not null)
        {
            updateData.Password = _hashService.HashString(updateData.Password);
        }
        
        return await _userRepository.UpdateAsync(id, updateData);
    }

    /// <summary>
    /// Метод авторизует пользователя по токену обновления.
    /// </summary>
    /// <param name="refreshToken">Токен обновления.</param>
    /// <returns>Пару JWT-токенов.</returns>
    /// <exception cref="IncorrectCreditionalException">Возникает в случае, если был введён некорректный токен обновления.</exception>
    public async Task<Result<(string accessToken, string refreshToken)>> LogInByRefreshTokenAsync(string refreshToken)
    {
        var parsedRefreshToken = await _jwtTokenService.ParseRefreshTokenAsync(refreshToken);
        if (parsedRefreshToken is null)
        {
            var incorrectCreditionalException = new IncorrectCreditionalException("Неверный RefreshToken");
            return new Result<(string accessToken, string refreshToken)>(incorrectCreditionalException);
        }

        var foundedUser = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (foundedUser is null)
        {
            var sessionExpiredException = new IncorrectCreditionalException("Сессия с введёным токеном истекла");
            return new Result<(string accessToken, string refreshToken)>(sessionExpiredException);
        }

        var tokenExpiredDate = parsedRefreshToken.IsLongSession
            ? DateTime.UtcNow.AddDays(30)
            : DateTime.UtcNow.AddMinutes(30);

        var newRefreshToken = _jwtTokenService.GenerateRefreshToken(new RefreshTokenDataModel
        {
            Login = foundedUser.Login,
            IsLongSession = parsedRefreshToken.IsLongSession
        }, tokenExpiredDate);
        
        var newAccessToken = _jwtTokenService.GenerateAccessToken(new UserDto
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        });

        var updateRefreshTokenError = await _userRepository.UpdateRefreshTokenAsync(foundedUser.Login, newRefreshToken);

        return updateRefreshTokenError.Match<Result<(string accessToken, string refreshToken)>>(exception =>
            new Result<(string accessToken, string refreshToken)>(exception), 
            () => (newAccessToken, newRefreshToken));
    }
}