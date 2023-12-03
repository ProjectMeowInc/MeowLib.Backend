using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Models;
using MeowLib.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с JWT-токенами.
/// </summary>
public class JwtTokensService : IJwtTokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private readonly SymmetricSecurityKey _accessTokenSecurityKey;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _inviteTokenSecurityKey;
    private readonly string _issuer;
    private readonly ILogger<JwtTokensService> _logger;
    private readonly SymmetricSecurityKey _refreshTokenSecurityKey;

    /// <summary>
    /// Конструктор.
    /// </summary>
    public JwtTokensService(ILogger<JwtTokensService> logger)
    {
        _logger = logger;
        // Init secret key
        var accessTokenSecurityKey = "QevAyHIKuOHJwG6sdYwnfrrbUW61cu4r3vuyzSNkBw1itzJD5AMXdKqLfzv"u8.ToArray();
        var refreshTokenSecurityKey = "Wv8HLBxBztPocFYSMDZn3074USr48gxw9RXZw4BCxAp290CqsPPG9frFLR2p"u8.ToArray();
        var inviteTokenSecurityKey = "OCGRroap9tNmWNxohF8i3ImZUWYxa4L64nJxDKnqt60lgqddND2kwVDZJaEA"u8.ToArray();

        _accessTokenSecurityKey = new SymmetricSecurityKey(accessTokenSecurityKey);
        _refreshTokenSecurityKey = new SymmetricSecurityKey(refreshTokenSecurityKey);
        _inviteTokenSecurityKey = new SymmetricSecurityKey(inviteTokenSecurityKey);

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
    /// Метод генерирует токен для приглашения в комманду.
    /// </summary>
    /// <param name="data">Информация для генерации токена.</param>
    /// <returns>Сгенерированный токен.</returns>
    public string GenerateInviteToTeamStringToken(InviteToTeamTokenModel data)
    {
        var expiredUnixDate = (long)data.InviteExpiredAt.Subtract(DateTime.UnixEpoch).TotalSeconds;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("userId", data.UserId.ToString()),
                new Claim("teamId", data.TeamId.ToString()),
                new Claim("inviteExpiredAt", expiredUnixDate.ToString())
            }),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(_inviteTokenSecurityKey, SecurityAlgorithms.HmacSha256Signature)
        };

        return WriteToken(tokenDescriptor);
    }

    /// <summary>
    /// Метод валидирует токен для приглашения в комманду.
    /// </summary>
    /// <param name="token">Токен для валидации.</param>
    /// <returns>Данные в токене в случае успеха, иначе - null.</returns>
    /// <exception cref="NullReferenceException">Возникает в случае если в валидном токене отсутствуют некоторые поля.</exception>
    public async Task<InviteToTeamTokenModel?> ParseInviteToTeamTokenAsync(string token)
    {
        var validateResult = await TokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _refreshTokenSecurityKey
        });

        if (!validateResult.IsValid)
        {
            return null;
        }

        var userId = int.Parse(validateResult.Claims["userId"].ToString() ??
                               throw new NullReferenceException("UserId не может быть null"));

        var teamId = int.Parse(validateResult.Claims["teamId"].ToString() ??
                               throw new NullReferenceException("TeamId не может быть null"));
        var inviteExpired = long.Parse(validateResult.Claims["inviteExpiredAt"].ToString() ??
                                       throw new NullReferenceException("InviteExpiredAt не может быть null"));

        // get as unix seconds. convert to DateTime
        var inviteExpiredDate = DateTime.UnixEpoch.AddSeconds(inviteExpired);

        return new InviteToTeamTokenModel
        {
            UserId = userId,
            TeamId = teamId,
            InviteExpiredAt = inviteExpiredDate
        };
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