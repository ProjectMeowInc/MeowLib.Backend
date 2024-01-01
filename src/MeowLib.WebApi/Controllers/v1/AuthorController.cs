using MeowLib.Domain.Author.Dto;
using MeowLib.Domain.Author.Services;
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
    /// Получение всех авторов.
    /// </summary>
    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await peopleService.GetAllAuthorsAsync();
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
    /// Создание нового автора.
    /// </summary>
    /// <param name="input">Данные для создания.</param>
    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var createResult = await peopleService.CreateAuthorAsync(input.Name);
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
    /// Обновление автора
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    /// <param name="input">Данные для обновления</param>
    [HttpPut("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] UpdateAuthorRequest input)
    {
        var updateResult = await peopleService.UpdateAuthorAsync(authorId, new PeopleDto
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
    /// Удаление автора.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    [HttpDelete("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int authorId)
    {
        var deleteAuthorResult = await peopleService.DeleteAuthorAsync(authorId);
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
    /// Получение автора.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    [HttpGet("{authorId}")]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorById([FromRoute] int authorId)
    {
        var foundedAuthor = await peopleService.GetAuthorByIdAsync(authorId);
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
    /// Получение автора по заданным параметрам.
    /// </summary>
    /// <param name="input">Параметры для поиска.</param>
    [HttpGet]
    [Route("search")]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorWithParams([FromQuery] GetAuthorWithFilterRequest input)
    {
        var getAuthorWithParamsResult = await peopleService.GetAuthorWithParams(input.Name);
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