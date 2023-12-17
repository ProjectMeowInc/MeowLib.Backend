using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Author;
using MeowLib.WebApi.Models.Responses.v1.Author;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/authors")]
public class AuthorController(IAuthorService authorService) : BaseController
{
    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await authorService.GetAllAuthorsAsync();
        return Json(new GetAllAuthorsResponse
        {
            Items = authors.Select(a => new AuthorModel
            {
                Id = a.Id,
                Name = a.Name
            })
        });
    }

    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var createResult = await authorService.CreateAuthorAsync(input.Name);
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

    [HttpPut("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] UpdateAuthorRequest input)
    {
        var updateResult = await authorService.UpdateAuthorAsync(authorId, new AuthorDto
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

    [HttpDelete("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int authorId)
    {
        var deleteAuthorResult = await authorService.DeleteAuthorAsync(authorId);
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

    [HttpGet("{authorId}")]
    [ProducesOkResponseType(typeof(AuthorModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorById([FromRoute] int authorId)
    {
        var foundedAuthor = await authorService.GetAuthorByIdAsync(authorId);
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

    [HttpGet]
    [Route("search")]
    [ProducesOkResponseType(typeof(GetAllAuthorsResponse))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorWithParams([FromQuery] GetAuthorWithFilterRequest input)
    {
        var getAuthorWithParamsResult = await authorService.GetAuthorWithParams(input.Name);
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