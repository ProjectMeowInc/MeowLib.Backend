using AutoMapper;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Author;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
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
    [ProducesOkResponseType(typeof(IEnumerable<AuthorDto>))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthorsAsync();
        return Json(authors);
    }

    [HttpPost, Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
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
                return validationException.ToResponse();
            }

            return ServerError();
        }

        var author = createResult.GetResult();
        return Json(author);
    }

    [HttpPut("{authorId:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(AuthorDto))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateAuthor([FromRoute] int authorId, [FromBody] UpdateAuthorRequest input)
    {
        var updateAuthorModel = _mapper.Map<UpdateAuthorRequest, UpdateAuthorEntityModel>(input);
        var updateResult = await _authorService.UpdateAuthorAsync(authorId, updateAuthorModel);

        if (updateResult.IsFailure)
        {
            var exception = updateResult.GetError();
            
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

        var updatedAuthor = updateResult.GetResult();
        return Json(updatedAuthor);
    }

    [HttpDelete("{authorId:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
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

    [HttpGet("{authorId:int}")]
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
        var getAuthorWithParamsResult = await _authorService.GetAuthorWithParams(input);
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