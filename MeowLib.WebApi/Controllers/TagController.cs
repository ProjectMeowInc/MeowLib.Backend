using AutoMapper;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Tag;
using MeowLib.Domain.Responses;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/tags")]
public class TagController : BaseController
{
    private readonly ITagService _tagService;
    private readonly IMapper _mapper;
    
    public TagController(ITagService tagService, IMapper mapper)
    {
        _tagService = tagService;
        _mapper = mapper;
    }

    [HttpPost, Authorization(RequiredRoles = new [] { UserRolesEnum.Admin })]
    [ProducesResponseType(200, Type = typeof(TagEntityModel))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest input)
    {
        try
        {
            var createModel = _mapper.Map<CreateTagRequest, CreateTagEntityModel>(input);
            var createdTag = await _tagService.CreateTagAsync(createModel);
            return Json(createdTag);
        }
        catch (ValidationException validationException)
        {
            var responseData = new ValidationErrorResponse(validationException.ValidationErrors);
            return Json(responseData, 403);
        }
        catch (ApiException apiException)
        {
            return Error(apiException.ErrorMessage);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> DeleteTag([FromRoute] int id)
    {
        try
        {
            var result = await _tagService.DeleteTagByIdAsync(id);
            if (!result)
            {
                return Error($"Тег с Id = {id} не найден", 404);
            }

            return Ok();
        }
        catch (ApiException apiException)
        {
            return Error(apiException.ErrorMessage);
        }
    }

    [HttpPut("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin })]
    [ProducesResponseType(200)]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(403, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateTag([FromRoute] int id, [FromBody] UpdateTagRequest input)
    {
        try
        {
            var updateModel = _mapper.Map<UpdateTagRequest, UpdateTagEntityModel>(input);
            var updatedTag = await _tagService.UpdateTagByIdAsync(id, updateModel);

            if (updatedTag is null)
            {
                return Error($"Тег с Id = {id} не найден", 404);
            }

            return Json(updatedTag);
        }
        catch (ValidationException validationException)
        {
            var responseData = new ValidationErrorResponse(validationException.ValidationErrors);
            return Json(responseData, 403);
        }
        catch (ApiException apiException)
        {
            return Error(apiException.ErrorMessage);
        }
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<TagDto>))]
    public async Task<ActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Json(tags);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200, Type = typeof(TagEntityModel))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetTagById([FromRoute] int id)
    {
        var foundedTag = await _tagService.GetTagByIdAsync(id);
        if (foundedTag is null)
        {
            return Error($"Тег с Id = {id} не найден", 404);
        }

        return Json(foundedTag);
    }
}