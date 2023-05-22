using AutoMapper;
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
        var createResult = await _authorService.CreateAuthorAsync(input.Name);
        return createResult.Match<ActionResult>(author => Json(author), exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            return ServerError();
        });
    }

    [HttpPut("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200, Type = typeof(AuthorDto))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int id, [FromBody] UpdateAuthorRequest input)
    {
        var updateAuthorModel = _mapper.Map<UpdateAuthorRequest, UpdateAuthorEntityModel>(input);
        var updateResult = await _authorService.UpdateAuthorAsync(id, updateAuthorModel);
        
        return updateResult.Match<ActionResult>(updatedAuthor => Json(updatedAuthor), exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            if (exception is ApiException)
            {
                return NotFoundError();
            }

            return ServerError();
        });
    }

    [HttpDelete("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200)]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> DeleteAuthor([FromRoute] int id)
    {
        var result = await _authorService.DeleteAuthorAsync(id);
        return result.Match<ActionResult>(ok =>
        {
            if (!ok)
            {
                return NotFoundError();
            }
            
            return Ok();
        }, _ => ServerError());
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