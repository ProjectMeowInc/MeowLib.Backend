using MeowLib.Domain.Exceptions.Team;
using MeowLib.Domain.Requests.Team;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.Team;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
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
}