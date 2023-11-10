using AutoMapper;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Tag;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/tags")]
public class TagController : BaseController
{
    private readonly ITagService _tagService;
    private readonly IMapper _mapper;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tagService">Сервис для работы с тегами.</param>
    /// <param name="mapper">Автомаппер.</param>
    public TagController(ITagService tagService, IMapper mapper)
    {
        _tagService = tagService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagEntityModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest input)
    {
        var createModel = _mapper.Map<CreateTagRequest, CreateTagEntityModel>(input);
        var createTagResult = await _tagService.CreateTagAsync(createModel);

        return createTagResult.Match<ActionResult>(createdTag => Json(createdTag), exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            return ServerError();
        });
    }

    [HttpDelete("{id:int}")]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteTag([FromRoute] int id)
    {
        var deleteTagResult = await _tagService.DeleteTagByIdAsync(id);

        return deleteTagResult.Match<ActionResult>(ok =>
        {
            if (!ok)
            {
                return NotFoundError();
            }

            return EmptyResult();
        }, _ => ServerError());
    }

    [HttpPut("{id:int}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagEntityModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateTag([FromRoute] int id, [FromBody] UpdateTagRequest input)
    {
        var updateModel = _mapper.Map<UpdateTagRequest, UpdateTagEntityModel>(input);
        var updateTagResult = await _tagService.UpdateTagByIdAsync(id, updateModel);

        return updateTagResult.Match<ActionResult>(updatedTag =>
        {
            if (updatedTag is null)
            {
                return NotFoundError();
            }

            return Json(updatedTag);
        }, exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            return ServerError();
        });
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(IEnumerable<TagDto>))]
    public async Task<ActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Json(tags);
    }

    [HttpGet("{id:int}")]
    [ProducesOkResponseType(typeof(TagEntityModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetTagById([FromRoute] int id)
    {
        var foundedTag = await _tagService.GetTagByIdAsync(id);
        if (foundedTag is null)
        {
            return NotFoundError();
        }

        return Json(foundedTag);
    }
}