using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Tag;
using MeowLib.WebApi.Models.Responses.v1.Tag;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[Route("api/v1/tags")]
public class TagController(ITagService tagService) : BaseController
{
    [HttpPost]
    [Authorization(RequiredRoles = new [] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagModel))]
    [ProducesForbiddenResponseType]
    public async Task<ActionResult> CreateTag([FromBody] CreateTagRequest input)
    {
        var createTagResult = await tagService.CreateTagAsync(input.Name, input.Description);

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
        return Json(new TagModel
        {
            Id = createdTag.Id,
            Name = createdTag.Name,
            Description = createdTag.Description
        });
    }

    [HttpDelete("{id}")]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> DeleteTag([FromRoute] int id)
    {
        var tagDeleted = await tagService.DeleteTagByIdAsync(id);

        if (!tagDeleted)
        {
            return NotFoundError();
        }

        return EmptyResult();
    }

    [HttpPut("{id}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateTag([FromRoute] int id, [FromBody] UpdateTagRequest input)
    {
        var updateTagResult = await tagService.UpdateTagByIdAsync(id, input.Name, input.Description);

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

        return Json(new TagModel
        {
            Id = updatedTag.Id,
            Name = updatedTag.Name,
            Description = updatedTag.Description
        });
    }

    [HttpGet]
    [ProducesOkResponseType(typeof(GetAllTagsResponse))]
    public async Task<ActionResult> GetAllTags()
    {
        var tags = await tagService.GetAllTagsAsync();
        return Json(new GetAllTagsResponse
        {
            Items = tags.Select(t => new TagModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description
            })
        });
    }

    [HttpGet("{id}")]
    [ProducesOkResponseType(typeof(TagModel))]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetTagById([FromRoute] int id)
    {
        var foundedTag = await tagService.GetTagByIdAsync(id);
        if (foundedTag is null)
        {
            return NotFoundError();
        }

        return Json(new TagModel
        {
            Id = foundedTag.Id,
            Name = foundedTag.Name,
            Description = foundedTag.Description
        });
    }
}