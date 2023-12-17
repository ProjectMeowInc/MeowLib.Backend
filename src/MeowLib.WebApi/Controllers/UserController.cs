using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.User;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/users")]
public class UserController : BaseController
{
    private readonly IUserService _userService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="userService">Сервис для взаимодействия с пользователями.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(IEnumerable<UserDto>))]
    public async Task<ActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Json(users);
    }

    [HttpPut("{id}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(UserDto))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest input)
    {
        var updateUserResult = await _userService.UpdateUser(id, new UpdateUserEntityModel
        {
            Login = input.Login,
            Password = input.Password,
            Role = input.Role
        });

        if (updateUserResult.IsFailure)
        {
            var exception = updateUserResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
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