using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.Tag;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/tags")]
public class TagController : BaseController
{
    private readonly ITagService _tagService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="tagService">Сервис для работы с тегами.</param>
    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost]
    [Authorization(RequiredRoles = new [] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagEntityModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest input)
    {
        var createTagResult = await _tagService.CreateTagAsync(input.Name, input.Description);

        if (createTagResult.IsFailure)
        {
            var exception = createTagResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var createdTag = createTagResult.GetResult();
        return Json(createdTag);
    }

    [HttpDelete("{id}")]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteTag([FromRoute] int id)
    {
        var tagDeleted = await _tagService.DeleteTagByIdAsync(id);

        if (!tagDeleted)
        {
            return NotFoundError();
        }

        return EmptyResult();
    }

    [HttpPut("{id}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagEntityModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateTag([FromRoute] int id, [FromBody] UpdateTagRequest input)
    {
        var updateTagResult = await _tagService.UpdateTagByIdAsync(id, input.Name, input.Description);

        if (updateTagResult.IsFailure)
        {
            var exception = updateTagResult.GetError();
            if (exception is ValidationException validationException)
            {
                return ValidationError(validationException.ValidationErrors);
            }

            return ServerError();
        }

        var updatedTag = updateTagResult.GetResult();
        if (updatedTag is null)
        {
            return NotFoundError();
        }

        return Json(updatedTag);
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(IEnumerable<TagDto>))]
    public async Task<ActionResult> GetAllTags()
    {
        var tags = await _tagService.GetAllTagsAsync();
        return Json(tags);
    }

    [HttpGet("{id}")]
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