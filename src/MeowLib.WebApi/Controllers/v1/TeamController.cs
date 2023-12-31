using MeowLib.Domain.Team.Exceptions;
using MeowLib.Domain.Team.Services;
using MeowLib.Domain.User.Exceptions;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Team;
using MeowLib.WebApi.Models.Responses.v1;
using MeowLib.WebApi.Models.Responses.v1.Team;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер комманд.
/// </summary>
/// <param name="teamService">Сервис комманд.</param>
/// <param name="logger">Логгер.</param>
[Route("api/v1/team")]
public class TeamController(ITeamService teamService, ILogger<TeamController> logger) : BaseController
{
    /// <summary>
    /// Создание новой комманды.
    /// </summary>
    /// <param name="payload">Данные для создания.</param>
    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<IActionResult> CreateNewTeam([FromBody] CreateTeamRequest payload)
    {
        var user = await GetUserDataAsync();

        var createNewTeamResult = await teamService.CreateNewTeamAsync(user.Id, payload.Name, payload.Description);
        if (createNewTeamResult.IsFailure)
        {
            var exception = createNewTeamResult.GetError();
            if (exception is TeamOwnerNotFoundException teamOwnerNotFoundException)
            {
                logger.LogError(teamOwnerNotFoundException.ErrorMessage);
                return UpdateAuthorizeError();
            }

            logger.LogError("Произошла неизвестная ошибка при создании команды: {error}", exception);
            return ServerError();
        }

        return EmptyResult();
    }

    /// <summary>
    /// Получение комманды.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    [HttpGet("{teamId}")]
    [ProducesOkResponseType(typeof(GetTeamByIdResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetTeamById([FromRoute] int teamId)
    {
        var foundedTeam = await teamService.GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return NotFoundError();
        }

        return Json(new GetTeamByIdResponse
        {
            Id = foundedTeam.Id,
            Name = foundedTeam.Name,
            Description = foundedTeam.Description,
            Members = foundedTeam.Members.Select(m => new TeamMemberModel
            {
                Id = m.User.Id,
                Login = m.User.Login,
                Role = m.Role
            })
        });
    }

    /// <summary>
    /// Установить роль пользователя в комманде.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="payload">Данные для установки.</param>
    [HttpPost("{teamId}/members/{userId}/role")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> SetUserTeamRole([FromRoute] int teamId, [FromRoute] int userId,
        [FromBody] SetUserTeamRoleRequest payload)
    {
        var requestFromUser = await GetUserDataAsync();
        var isCanChange = await teamService.CheckUserIsTeamAdminAsync(teamId, requestFromUser.Id);
        if (!isCanChange)
        {
            logger.LogWarning(
                "Пользователь с Id = {userId} не имеет доступа к изменению ролей в команде с Id = {teamId}",
                requestFromUser.Id, teamId);
            return Error("У вас нет доступа к изменению ролей в данной команде", 400);
        }

        var setUserTeamRoleResult = await teamService.SetUserTeamRoleAsync(teamId, userId, payload.NewRole);
        if (setUserTeamRoleResult.IsFailure)
        {
            var exception = setUserTeamRoleResult.GetError();
            if (exception is TeamNotFoundException)
            {
                logger.LogWarning("Команда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is ChangeOwnerRoleException)
            {
                logger.LogWarning("Попытка изменить роль владельца команды");
                return Error("Нельзя изменить роль владельца команды", 400);
            }

            if (exception is UserNotFoundException)
            {
                logger.LogWarning("Запрашиваемый пользователь не состоит в команде");
                return Error("Пользователь не состоит в команде");
            }

            logger.LogError("Неизвестная ошибка обновления командной роли пользователя: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    /// <summary>
    /// Покинуть комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    [HttpPost("{teamId}/leave")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> LeaveFromTeam([FromRoute] int teamId)
    {
        var requestUser = await GetUserDataAsync();

        var removeUserResult = await teamService.RemoveFromTeamAsync(teamId, requestUser.Id);
        if (removeUserResult.IsFailure)
        {
            var exception = removeUserResult.GetError();
            if (exception is TeamNotFoundException)
            {
                logger.LogWarning("Команда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is ChangeOwnerRoleException)
            {
                logger.LogWarning("Попытка выйти из команды владельцем команды");
                return Error("Невозможно покинуть команду будучи её владельцем", 400);
            }

            if (exception is UserNotFoundException)
            {
                logger.LogWarning("Попытка покинуть команду в который пользователь не состоит");
                return Error("Вы не состоите в этой команде", 400);
            }

            logger.LogError("Неизвестная ошибка покидания команды: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    /// <summary>
    /// Пригласить пользователя в комманду.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    [HttpPost("{teamId}/members/invite/{userId}")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> InviteUserToTeam([FromRoute] int teamId, [FromRoute] int userId)
    {
        var requestUser = await GetUserDataAsync();

        var hasAdminAccess = await teamService.CheckUserIsTeamAdminAsync(teamId, requestUser.Id);
        if (!hasAdminAccess)
        {
            return Error("У вас нет доступа к добавлению пользователей в комманду", 400);
        }

        var inviteResult = await teamService.InviteUserToTeamAsync(teamId, userId);
        if (inviteResult.IsFailure)
        {
            var exception = inviteResult.GetError();
            if (exception is TeamNotFoundException)
            {
                logger.LogWarning("Комманда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is UserNotFoundException)
            {
                logger.LogWarning("Пользователь с Id = {userId} не найден", userId);
                return Error("Запрашиваемый пользователь не найден", 400);
            }

            if (exception is UserAlreadyInTeamException)
            {
                logger.LogWarning("Пользователь с Id = {userId} уже состоит в комманде с Id = {teamId}",
                    userId, teamId);
                return Error("Запрашиваемый пользователь уже состоит в комманде", 400);
            }

            logger.LogError("Произошла неизвестная ошибка при приглашении пользователя в комманду: {exception}",
                exception);
            return ServerError();
        }

        return Ok();
    }

    /// <summary>
    /// Удалить пользователя из комманды.
    /// </summary>
    /// <param name="teamId">Id комманды.</param>
    /// <param name="userId">Id пользователя.</param>
    [HttpPost("{teamId}/members/{userId}/remove")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> RemoveUserFromTeam([FromRoute] int teamId, [FromRoute] int userId)
    {
        var fromUserData = await GetUserDataAsync();

        var isUserAdmin = await teamService.CheckUserIsTeamAdminAsync(teamId, fromUserData.Id);
        if (!isUserAdmin)
        {
            logger.LogWarning(
                "Пользователь с Id = {userId} пытался исключить участника с Id = {teamMemberId} из комманды не имея на это прав",
                fromUserData.Id, userId);
            return Error("У вас нет доступа к исключению участников из комманды", 400);
        }

        var removeFromTeamResult = await teamService.RemoveFromTeamAsync(teamId, userId);
        if (removeFromTeamResult.IsFailure)
        {
            var exception = removeFromTeamResult.GetError();

            if (exception is TeamNotFoundException)
            {
                return NotFoundError();
            }

            if (exception is ChangeOwnerRoleException)
            {
                return Error("Невозможно исключить владельца комманды", 400);
            }

            if (exception is UserNotFoundException)
            {
                return Error("Пользователь не состоит в комманде", 400);
            }

            logger.LogError("Неизвестная ошибка при исключении пользователя из комманды: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }
}