using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Result;

namespace MeowLIb.WebApi.Services.Interface;

public interface ITeamService
{
    /// <summary>
    /// Метод создаёт новую команду.
    /// </summary>
    /// <param name="createdById">Id пользователя запросившего создание команды.</param>
    /// <param name="name">Название команды.</param>
    /// <param name="description">Описание команды.</param>
    /// <returns>Модель команды в случае успеха</returns>
    /// <exception cref="TeamOwnerNotFoundException">Возникает в случае, если пользователь запросивший создание команды не найден</exception>
    Task<Result<TeamEntityModel>> CreateNewTeamAsync(int createdById, string name, string description);
    
    /// <summary>
    /// Метод вовзращает информацию о команде по её Id.
    /// </summary>
    /// <param name="teamId">Id команды.</param>
    /// <returns>Модель команды, если она была найдена. Иначе - null</returns>
    Task<TeamEntityModel?> GetTeamByIdAsync(int teamId);
}