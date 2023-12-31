using MeowLib.Domain.Shared.Models;

namespace MeowLib.Services.Interface;

public interface INotificationTokenService
{
    /// <summary>
    /// Метод генерирует токен для приглашения в комманду.
    /// </summary>
    /// <param name="data">Информация для генерации токена.</param>
    /// <returns>Сгенерированный токен.</returns>
    string GenerateInviteToTeamStringToken(InviteToTeamTokenModel data);

    /// <summary>
    /// Метод валидирует токен для приглашения в комманду.
    /// </summary>
    /// <param name="token">Токен для валидации.</param>
    /// <returns>Данные в токене в случае успеха, иначе - null.</returns>
    /// <exception cref="NullReferenceException">Возникает в случае если в валидном токене отсутствуют некоторые поля.</exception>
    Task<InviteToTeamTokenModel?> ParseInviteToTeamTokenAsync(string token);
}