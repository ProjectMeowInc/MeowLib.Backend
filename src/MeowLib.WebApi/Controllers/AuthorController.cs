using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.Author;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/authors")]
public class AuthorController : BaseController
{
    private readonly IAuthorService _authorService;

    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(IEnumerable<AuthorDto>))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Json(authors);
    }

    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorDto))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var createResult = await _authorService.CreateAuthorAsync(input.Name);
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
        return Json(author);
    }

    [HttpPut("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorDto))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] UpdateAuthorRequest input)
    {
        var updateResult = await _authorService.UpdateAuthorAsync(authorId, new UpdateAuthorEntityModel
        {
            Name = input.Name
        });

        if (updateResult.IsFailure)
        {
            var exception = updateResult.GetError();

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

        var updatedAuthor = updateResult.GetResult();
        return Json(updatedAuthor);
    }

    [HttpDelete("{authorId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int authorId)
    {
        var deleteAuthorResult = await _authorService.DeleteAuthorAsync(authorId);
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
    [ProducesOkResponseType(typeof(AuthorDto))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorById([FromRoute] int authorId)
    {
        var result = await _authorService.GetAuthorByIdAsync(authorId);
        if (result.IsFailure)
        {
            var exception = result.GetError();
            if (exception is EntityNotFoundException)
            {
                return NotFoundError();
            }

            return ServerError();
        }

        var author = result.GetResult();
        return Json(author);
    }

    [HttpPost]
    [Route("get-with-params")]
    [ProducesOkResponseType(typeof(IEnumerable<AuthorDto>))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetAuthorWithParams([FromBody] GetAuthorWithParamsRequest input)
    {
        var getAuthorWithParamsResult = await _authorService.GetAuthorWithParams(input.Name);
        if (getAuthorWithParamsResult.IsFailure)
        {
            var exception = getAuthorWithParamsResult.GetError();
            if (exception is SearchNotFoundException)
            {
                return Error("Авторы с заданным параметрами не найдены", 404);
            }

            return ServerError();
        }

        var author = getAuthorWithParamsResult.GetResult();
        return Json(author);
    }
}