using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.User.Enums;
using MeowLib.Domain.User.Services;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.User;
using MeowLib.WebApi.Models.Responses.v1.User;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер пользователей.
/// </summary>
/// <param name="userService">Сервис пользователей.</param>
[Route("api/v1/users")]
public class UserController(IUserService userService) : BaseController
{
    /// <summary>
    /// Получение всех пользователей.
    /// </summary>
    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllUsersResponse))]
    public async Task<ActionResult> GetAll()
    {
        var users = await userService.GetAllAsync();
        return Json(new GetAllUsersResponse
        {
            Items = users.Select(u => new UserModel
            {
                Id = u.Id,
                Login = u.Login,
                Role = u.Role
            })
        });
    }

    /// <summary>
    /// Обновление пользователя.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    /// <param name="input">Данные для обновления.</param>
    [HttpPut("{userId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(UserModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateUser([FromRoute] int userId, [FromBody] UpdateUserRequest input)
    {
        var updateUserResult = await userService.UpdateUser(userId, input.Login, input.Password);

        if (updateUserResult.IsFailure)
        {
            var exception = updateUserResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var updatedUser = updateUserResult.GetResult();
        if (updatedUser is null)
        {
            return NotFoundError();
        }

        return Json(new UserModel
        {
            Id = updatedUser.Id,
            Login = updatedUser.Login,
            Role = updatedUser.Role
        });
    }

    /// <summary>
    /// Получить пользователя по Id.
    /// </summary>
    /// <param name="userId">Id пользователя.</param>
    [HttpGet("{userId}")]
    [ProducesOkResponseType(typeof(UserModel))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] int userId)
    {
        var foundedUser = await userService.GetUserByIdAsync(userId);
        if (foundedUser is null)
        {
            return NotFoundError();
        }

        return Ok(new UserModel
        {
            Id = foundedUser.Id,
            Login = foundedUser.Login,
            Role = foundedUser.Role
        });
    }
}