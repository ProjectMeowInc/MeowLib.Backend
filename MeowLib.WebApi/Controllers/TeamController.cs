using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Exceptions.User;
using MeowLib.Domain.Requests.Team;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.Team;
using MeowLIb.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
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

    [HttpPost, Authorization]
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

    [HttpPost("{teamId}/members/{userId}/role"), Authorization]
    [ProducesOkResponseType]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> SetUserTeamRole([FromRoute] int teamId, [FromRoute] int userId, 
        [FromBody] SetUserTeamRoleRequest payload)
    {
        var requestFromUser = await GetUserDataAsync();
        var isCanChange = await _teamService.CheckIsUserCanChangeTeamRoleAsync(teamId, requestFromUser.Id);
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

    [HttpPost("{teamId}/leave"), Authorization]
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
}