using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.IdentityModel.Tokens;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с JWT-токенами.
/// </summary>
public class JwtTokensService : IJwtTokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private readonly SymmetricSecurityKey _securityKey;
    private readonly string _issuer;
    private readonly string _audience;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    public JwtTokensService()
    {
        // Init secret key
        var secretString = "askgjiou21SAJDKSAJLDJasdasd";
        var secretKey = Encoding.UTF8.GetBytes(secretString);
        _securityKey = new SymmetricSecurityKey(secretKey);
        
        _issuer = "MeowLib";
        _audience = "MeowLibUser";
    }

    /// <summary>
    /// Токен генерирует JWT-токен для авторизации пользователя.
    /// </summary>
    /// <param name="userData">Данные о пользователе.</param>
    /// <returns>JWT-токен в виде строки.</returns>
    public string GenerateToken(UserDto userData)
    {
        var tokenDescription = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", userData.Id.ToString()),
                new Claim("Login", userData.Login),
                new Claim("Role", userData.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature),
        };

        var token = TokenHandler.CreateToken(tokenDescription);
        return TokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Парсит токен и возвращает информацию хранащуюся в нём.
    /// </summary>
    /// <param name="token">Токен.</param>
    /// <returns>Информация о пользователе в случае удачного парсинга, иначе - null</returns>
    public async Task<UserDto?> ParseTokenAsync(string token)
    {
        var tokenValidationResult = await TokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = _securityKey
        });

        if (!tokenValidationResult.IsValid)
        {
            return null;
        }

        var claims = tokenValidationResult.Claims;

        var expiredAtObject = claims["exp"];
        if (expiredAtObject is not int expiredAtInt)
        {
            // TODO: ADD LOG
            return null;
        }
        
        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expiredAtInt).UtcDateTime;

        if (expirationTime < DateTime.UtcNow)
        {
            return null;
        }
        
        var userData = new UserDto
        {
            Id = int.Parse((string)claims["Id"]),
            Login = (string)claims["Login"],
            Role = Enum.Parse<UserRolesEnum>((string)claims["Role"])
        };

        return userData;
    }
}