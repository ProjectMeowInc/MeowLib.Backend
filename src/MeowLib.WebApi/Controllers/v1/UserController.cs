using MeowLib.Domain.Dto.User;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.User;
using MeowLib.WebApi.Models.Responses.v1.User;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/users")]
public class UserController(IUserService userService) : BaseController
{
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

    [HttpPut("{id}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(UserModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserRequest input)
    {
        var updateUserResult = await userService.UpdateUser(id, input.Login, input.Password);

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
}