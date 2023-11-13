using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Models;
using MeowLIb.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MeowLIb.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с JWT-токенами.
/// </summary>
public class JwtTokensService : IJwtTokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private readonly SymmetricSecurityKey _accessTokenSecurityKey;
    private readonly SymmetricSecurityKey _refreshTokenSecurityKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ILogger<JwtTokensService> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public JwtTokensService(ILogger<JwtTokensService> logger)
    {
        _logger = logger;
        // Init secret key
        var accessTokenSecurityKey = "QevAyHIKuOHJwG6sdYwnfrrbUW61cu4r3vuyzSNkBw1itzJD5AMXdKqLfzv"u8.ToArray();
        var refreshTokenSecurityKey = "Wv8HLBxBztPocFYSMDZn3074USr48gxw9RXZw4BCxAp290CqsPPG9frFLR2p"u8.ToArray();

        _accessTokenSecurityKey = new SymmetricSecurityKey(accessTokenSecurityKey);
        _refreshTokenSecurityKey = new SymmetricSecurityKey(refreshTokenSecurityKey);

        _issuer = "MeowLib";
        _audience = "MeowLibUser";
    }

    /// <summary>
    /// Метод генерирует JWT-токен доступа.
    /// </summary>
    /// <param name="userData">Данные о пользователе.</param>
    /// <returns>JWT-токен в виде строки.</returns>
    public string GenerateAccessToken(UserDto userData)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", userData.Id.ToString()),
                new Claim("login", userData.Login),
                new Claim("userRole", userData.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(_accessTokenSecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Метод генерирует JWT-токен обновления.
    /// </summary>
    /// <param name="tokenData">Данные для записи в токен.</param>
    /// <param name="expiredAt">Время истечения токена обновления.</param>
    /// <returns>Токен в виде строки.</returns>
    public string GenerateRefreshToken(RefreshTokenDataModel tokenData, DateTime expiredAt)
    {
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("login", tokenData.Login),
                new Claim("isLongSession", tokenData.IsLongSession.ToString())
            }),
            Expires = expiredAt,
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials =
                new SigningCredentials(_refreshTokenSecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return WriteToken(tokenDescription);
    }

    /// <summary>
    /// Парсит JWT-токен доступа и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <returns>Информация о пользователе в случае удачного парсинга, иначе - null</returns>
    public async Task<UserDto?> ParseAccessTokenAsync(string token)
    {
        var tokenValidationResult = await TokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _accessTokenSecurityKey
        });

        if (!tokenValidationResult.IsValid)
        {
            return null;
        }

        var claims = tokenValidationResult.Claims;

        var expiredAtObject = claims["exp"];
        if (expiredAtObject is not int expiredAtInt)
        {
            _logger.LogError("Ошибка получения даты истечения из токена");
            return null;
        }

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expiredAtInt).UtcDateTime;

        if (expirationTime < DateTime.UtcNow)
        {
            return null;
        }

        var userData = new UserDto
        {
            Id = int.Parse((string)claims["id"]),
            Login = (string)claims["login"],
            Role = Enum.Parse<UserRolesEnum>((string)claims["userRole"])
        };

        return userData;
    }

    /// <summary>
    /// Метод парсит JWT-токет обновления и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен обновления.</param>
    /// <returns>Информация в токене обновления.</returns>
    public async Task<RefreshTokenDataModel?> ParseRefreshTokenAsync(string token)
    {
        var tokenValidationResult = await TokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _refreshTokenSecurityKey
        });

        if (!tokenValidationResult.IsValid)
        {
            return null;
        }

        var claims = tokenValidationResult.Claims;

        var tokenData = new RefreshTokenDataModel
        {
            Login = (string)claims["login"],
            IsLongSession = bool.Parse((string)claims["isLongSession"])
        };

        return tokenData;
    }

    /// <summary>
    /// Метод подписывает JWT-токен.
    /// </summary>
    /// <param name="tokenDescriptor">Токен для подписания.</param>
    /// <returns>Строка в виде JWT-токена.</returns>
    private string WriteToken(SecurityTokenDescriptor tokenDescriptor)
    {
        return TokenHandler.WriteToken(TokenHandler.CreateToken(tokenDescriptor));
    }
}