using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MeowLib.Domain.Shared.Models;
using MeowLib.Services.Interface;
using Microsoft.IdentityModel.Tokens;

namespace MeowLib.Services.Implementation.Production;

public class NotificationTokenService : INotificationTokenService
{
    private static readonly JwtSecurityTokenHandler TokenHandler = new();
    private readonly string _audience;
    private readonly SymmetricSecurityKey _inviteTokenSecurityKey;
    private readonly string _issuer;

    public NotificationTokenService()
    {
        var inviteTokenSecurityKey = "OCGRroap9tNmWNxohF8i3ImZUWYxa4L64nJxDKnqt60lgqddND2kwVDZJaEA"u8.ToArray();
        _inviteTokenSecurityKey = new SymmetricSecurityKey(inviteTokenSecurityKey);

        _issuer = "MeowLib";
        _audience = "MeowLibUser";
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
            IssuerSigningKey = _inviteTokenSecurityKey
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