using MeowLib.Domain.People.Dto;
using MeowLib.Domain.People.Services;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.User.Enums;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Author;
using MeowLib.WebApi.Models.Responses.v1.Author;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер авторов.
/// </summary>
/// <param name="peopleService">Сервис авторов</param>
[Route("api/v1/authors")]
public class AuthorController(IPeopleService peopleService) : BaseController
{
    /// <summary>
    /// [DEPRECATED] Получение всех авторов.
    /// </summary>
    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await peopleService.GetAllPeoplesAsync();
        return Json(new GetAllAuthorsResponse
        {
            Items = authors.Select(a => new AuthorModel
            {
                Id = a.Id,
                Name = a.Name
            })
        });
    }

    /// <summary>
    /// [DEPRECATED] Создание нового автора.
    /// </summary>
    /// <param name="input">Данные для создания.</param>
    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var createResult = await peopleService.CreatePeopleAsync(input.Name);
        if (createResult.IsFailure)
        {
            var exception = createResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var author = createResult.GetResult();
        return Json(new AuthorModel
        {
            Id = author.Id,
            Name = author.Name
        });
    }

    /// <summary>
    /// [DEPRECATED] Обновление автора
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    /// <param name="input">Данные для обновления</param>
    [HttpPut("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] UpdateAuthorRequest input)
    {
        var updateResult = await peopleService.UpdatePeopleAsync(authorId, new PeopleDto
        {
            Id = authorId,
            Name = input.Name
        });

        if (updateResult.IsFailure)
        {
            var exception = updateResult.GetError();

            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var updatedAuthor = updateResult.GetResult();
        if (updatedAuthor is null)
        {
            return NotFoundError();
        }

        return Json(new AuthorModel
        {
            Id = updatedAuthor.Id,
            Name = updatedAuthor.Name
        });
    }

    /// <summary>
    /// [DEPRECATED] Удаление автора.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    [HttpDelete("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int authorId)
    {
        var deleteAuthorResult = await peopleService.DeletePeopleAsync(authorId);
        if (deleteAuthorResult.IsFailure)
        {
            return ServerError();
        }

        var isFounded = deleteAuthorResult.GetResult();
        if (!isFounded)
        {
            return NotFoundError();
        }

        return Ok();
    }

    /// <summary>
    /// [DEPRECATED] Получение автора.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    [HttpGet("{authorId}")]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> GetAuthorById([FromRoute] int authorId)
    {
        var foundedAuthor = await peopleService.GetPeopleByIdAsync(authorId);
        if (foundedAuthor is null)
        {
            return NotFoundError();
        }

        return Json(new AuthorModel
        {
            Id = foundedAuthor.Id,
            Name = foundedAuthor.Name
        });
    }

    /// <summary>
    /// [DEPRECATED] Получение автора по заданным параметрам.
    /// </summary>
    /// <param name="input">Параметры для поиска.</param>
    [HttpGet]
    [Route("search")]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    [ProducesNotFoundResponseType]
    [DeprecatedMethod(10, 2, 2024)]
    public async Task<ActionResult> GetAuthorWithParams([FromQuery] GetAuthorWithFilterRequest input)
    {
        var getAuthorWithParamsResult = await peopleService.GetPeopleWithParams(input.Name);
        if (getAuthorWithParamsResult.IsFailure)
        {
            var exception = getAuthorWithParamsResult.GetError();
            if (exception is SearchNotFoundException)
            {
                return Error("Авторы с заданным параметрами не найдены", 404);
            }

            return ServerError();
        }

        var authors = getAuthorWithParamsResult.GetResult();
        return Json(new GetAllAuthorsResponse
        {
            Items = authors.Select(a => new AuthorModel
            {
                Id = a.Id,
                Name = a.Name
            })
        });
    }
}