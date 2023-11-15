using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.Team;
using MeowLib.WebApi.Models.Responses;
using MeowLib.WebApi.Models.Responses.Team;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/team")]
public class TeamController : BaseController
{
    private readonly ITeamService _teamService;
    private readonly ILogger<TeamController> _logger;

    public TeamController(ITeamService teamService, ILogger<TeamController> logger)
    {
        _teamService = teamService;
        _logger = logger;
    }

    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(401, Type = typeof(BaseErrorResponse))]
    public async Task<IActionResult> CreateNewTeam([FromBody] CreateTeamRequest payload)
    {
        var user = await GetUserDataAsync();

        var createNewTeamResult = await _teamService.CreateNewTeamAsync(user.Id, payload.Name, payload.Description);
        if (createNewTeamResult.IsFailure)
        {
            var exception = createNewTeamResult.GetError();
            if (exception is TeamOwnerNotFoundException teamOwnerNotFoundException)
            {
                _logger.LogError(teamOwnerNotFoundException.ErrorMessage);
                return UpdateAuthorizeError();
            }

            _logger.LogError("Произошла неизвестная ошибка при создании команды: {error}", exception);
            return ServerError();
        }

        return EmptyResult();
    }

    [HttpGet("{teamId}")]
    [ProducesOkResponseType(typeof(GetTeamByIdResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetTeamById([FromRoute] int teamId)
    {
        var foundedTeam = await _teamService.GetTeamByIdAsync(teamId);
        if (foundedTeam is null)
        {
            return NotFoundError();
        }

        return Json(new GetTeamByIdResponse
        {
            Id = foundedTeam.Id,
            Name = foundedTeam.Name,
            Description = foundedTeam.Description,
            Members = foundedTeam.Members.Select(m => new TeamMember
            {
                Id = m.User.Id,
                Login = m.User.Login,
                Role = m.Role
            })
        });
    }

    [HttpPost("{teamId}/members/{userId}/role")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> SetUserTeamRole([FromRoute] int teamId, [FromRoute] int userId,
        [FromBody] SetUserTeamRoleRequest payload)
    {
        var requestFromUser = await GetUserDataAsync();
        var isCanChange = await _teamService.CheckUserIsTeamAdminAsync(teamId, requestFromUser.Id);
        if (!isCanChange)
        {
            _logger.LogWarning(
                "Пользователь с Id = {userId} не имеет доступа к изменению ролей в команде с Id = {teamId}",
                requestFromUser.Id, teamId);
            return Error("У вас нет доступа к изменению ролей в данной команде", 400);
        }

        var setUserTeamRoleResult = await _teamService.SetUserTeamRoleAsync(teamId, userId, payload.NewRole);
        if (setUserTeamRoleResult.IsFailure)
        {
            var exception = setUserTeamRoleResult.GetError();
            if (exception is TeamNotFoundException)
            {
                _logger.LogWarning("Команда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is ChangeOwnerRoleException)
            {
                _logger.LogWarning("Попытка изменить роль владельца команды");
                return Error("Нельзя изменить роль владельца команды", 400);
            }

            if (exception is UserNotFoundException)
            {
                _logger.LogWarning("Запрашиваемый пользователь не состоит в команде");
                return Error("Пользователь не состоит в команде");
            }

            _logger.LogError("Неизвестная ошибка обновления командной роли пользователя: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    [HttpPost("{teamId}/leave")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> LeaveFromTeam([FromRoute] int teamId)
    {
        var requestUser = await GetUserDataAsync();

        var removeUserResult = await _teamService.RemoveFromTeamAsync(teamId, requestUser.Id);
        if (removeUserResult.IsFailure)
        {
            var exception = removeUserResult.GetError();
            if (exception is TeamNotFoundException)
            {
                _logger.LogWarning("Команда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is ChangeOwnerRoleException)
            {
                _logger.LogWarning("Попытка выйти из команды владельцем команды");
                return Error("Невозможно покинуть команду будучи её владельцем", 400);
            }

            if (exception is UserNotFoundException)
            {
                _logger.LogWarning("Попытка покинуть команду в который пользователь не состоит");
                return Error("Вы не состоите в этой команде", 400);
            }

            _logger.LogError("Неизвестная ошибка покидания команды: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    [HttpPost("{teamId}/invite/{userId}")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> InviteUserToTeam([FromRoute] int teamId, [FromRoute] int userId)
    {
        var requestUser = await GetUserDataAsync();

        var hasAdminAccess = await _teamService.CheckUserIsTeamAdminAsync(teamId, requestUser.Id);
        if (!hasAdminAccess)
        {
            return Error("У вас нет доступа к добавлению пользователей в комманду", 400);
        }

        var inviteResult = await _teamService.InviteUserToTeamAsync(teamId, userId);
        if (inviteResult.IsFailure)
        {
            var exception = inviteResult.GetError();
            if (exception is TeamNotFoundException)
            {
                _logger.LogWarning("Комманда с Id = {teamId} не найдена", teamId);
                return NotFoundError();
            }

            if (exception is UserNotFoundException)
            {
                _logger.LogWarning("Пользователь с Id = {userId} не найден", userId);
                return Error("Запрашиваемый пользователь не найден", 400);
            }

            if (exception is UserAlreadyInTeamException)
            {
                _logger.LogWarning("Пользователь с Id = {userId} уже состоит в комманде с Id = {teamId}",
                    userId, teamId);
                return Error("Запрашиваемый пользователь уже состоит в комманде", 400);
            }

            _logger.LogError("Произошла неизвестная ошибка при приглашении пользователя в комманду: {exception}",
                exception);
            return ServerError();
        }

        return Ok();
    }

    [HttpPost("{teamId}/members/{userId}/remove")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> RemoveUserFromTeam([FromRoute] int teamId, [FromRoute] int userId)
    {
        var fromUserData = await GetUserDataAsync();

        var isUserAdmin = await _teamService.CheckUserIsTeamAdminAsync(teamId, fromUserData.Id);
        if (!isUserAdmin)
        {
            _logger.LogWarning(
                "Пользователь с Id = {userId} пытался исключить участника с Id = {teamMemberId} из комманды не имея на это прав",
                fromUserData.Id, userId);
            return Error("У вас нет доступа к исключению участников из комманды", 400);
        }

        var removeFromTeamResult = await _teamService.RemoveFromTeamAsync(teamId, userId);
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

            _logger.LogError("Неизвестная ошибка при исключении пользователя из комманды: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }
}