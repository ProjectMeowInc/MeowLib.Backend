using AutoMapper;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.User;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userService">Сервис для взаимодействия с пользователями.</param>
    /// <param name="mapper">Автомаппер.</param>
    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(IEnumerable<UserDto>))]
    public async Task<ActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Json(users);
    }

    [HttpPut("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(UserDto))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest input)
    {
        var mappedRequest = _mapper.Map<UpdateUserRequest, UpdateUserEntityModel>(input);
        
        var updateUserResult = await _userService.UpdateUser(id, mappedRequest);
        if (updateUserResult.IsFailure)
        {
            var exception = updateUserResult.GetError();
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            if (exception is EntityNotFoundException)
            {
                return NotFoundError();
            }

            return ServerError();
        }

        var updatedUser = updateUserResult.GetResult();
        return Json(updatedUser);
    }
}