using AutoMapper;
using LanguageExt.Pipes;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Requests.Author;
using MeowLib.Domain.Responses;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using ValidationException = MeowLib.Domain.Exceptions.Services.ValidationException;

namespace MeowLib.WebApi.Controllers;

[Route("api/authors")]
public class AuthorController : BaseController
{
    private readonly IAuthorService _authorService;
    private readonly IMapper _mapper;
    
    public AuthorController(IAuthorService authorService, IMapper mapper)
    {
        _authorService = authorService;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Json(authors);
    }

    [HttpPost, Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200, Type = typeof(AuthorDto))]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var author = await _authorService.CreateAuthorAsync(input.Name);
        return Json(author);
    }

    [HttpPut("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200, Type = typeof(AuthorDto))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int id, [FromBody] UpdateAuthorRequest input)
    {
        var updateAuthorModel = _mapper.Map<UpdateAuthorRequest, UpdateAuthorEntityModel>(input);
        try
        {
            var updatedAuthor = await _authorService.UpdateAuthorAsync(id, updateAuthorModel);
            return Json(updatedAuthor);
        }
        catch (ValidationException validationException)
        {
            var responseData = new ValidationErrorResponse(validationException.ValidationErrors);
            return Json(responseData, 403);
        }
        catch (ApiException apiException)
        {
            return Error(apiException.ErrorMessage, 404);
        }
    }

    [HttpDelete("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200)]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int id)
    {
        try
        {
            var isDeleted = await _authorService.DeleteAuthorAsync(id);
            if (!isDeleted)
            {
                return Error($"Автор с Id = {id} не найден", 404);
            }

            return Ok();
        }
        catch (ApiException apiException)
        {
            return Error(apiException.ErrorMessage, 500);
        }
    }

    [HttpGet("{authorId:int}")]
    public async Task<ActionResult> GetAuthorById([FromRoute] int authorId)
    {
        var result = await _authorService.GetAuthorByIdAsync(authorId);
        return result.Match<ActionResult>(author => Json(author), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return NotFoundError();
            }

            return ServerError();
        });
    }
}