using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Result;

namespace MeowLIb.Services.Interface;

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

    /// <summary>
    /// Метод устанавливает роль пользователя в команде.
    /// </summary>
    /// <param name="teamId">Id команды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="role">Новая роль.</param>
    /// <returns>Результат выполнения операции.</returns>
    /// <exception cref="TeamNotFoundException">Возникает в случае, если команда с запрашиваемым Id не была найдена.</exception>
    /// <exception cref="ChangeOwnerRoleException">Возникает в случае, если роль попытались сменить у владельца.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователя нет в команде.</exception>
    Task<Result> SetUserTeamRoleAsync(int teamId, int userId, UserTeamMemberRoleEnum role);

    /// <summary>
    /// Метод проверяет есть ли у пользователя доступ админ-функциям в команде.
    /// </summary>
    /// <param name="teamId">Id команды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>True - если имеет доступ, иначе - false</returns>
    Task<bool> CheckUserIsTeamAdminAsync(int teamId, int userId);

    /// <summary>
    /// Метод удаляет пользователя из команды.
    /// </summary>
    /// <param name="teamId">Id команды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <returns>Результат удаления.</returns>
    /// <exception cref="TeamNotFoundException">Возникает в случае, если команда не найдена</exception>
    /// <exception cref="ChangeOwnerRoleException">Возникает в попытке удалить владельца команды из команды.</exception>
    /// <exception cref="UserNotFoundException">Возникает в случае, если пользователь не состоит в заданной команде.</exception>
    Task<Result> RemoveFromTeamAsync(int teamId, int userId);
}