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

/// <summary>
/// Контроллер тегов.
/// </summary>
/// <param name="tagService">Сервис тегов.</param>
[Route("api/v1/tags")]
public class TagController(ITagService tagService) : BaseController
{
    /// <summary>
    /// Создание тега.
    /// </summary>
    /// <param name="input">Данные для создания.</param>
    [HttpPost]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
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

    /// <summary>
    /// Удаление тега.
    /// </summary>
    /// <param name="tagId">Id тега.</param>
    [HttpDelete("{tagId}")]
    [ProducesOkResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> DeleteTag([FromRoute] int tagId)
    {
        var tagDeleted = await tagService.DeleteTagByIdAsync(tagId);

        if (!tagDeleted)
        {
            return NotFoundError();
        }

        return EmptyResult();
    }

    /// <summary>
    /// Обновление тега.
    /// </summary>
    /// <param name="tagId">Id тега.</param>
    /// <param name="input">Данные для обновления.</param>
    [HttpPut("{tagId}")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin })]
    [ProducesOkResponseType(typeof(TagModel))]
    [ProducesForbiddenResponseType]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> UpdateTag([FromRoute] int tagId, [FromBody] UpdateTagRequest input)
    {
        var updateTagResult = await tagService.UpdateTagByIdAsync(tagId, input.Name, input.Description);

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

    /// <summary>
    /// Получение всех тегов.
    /// </summary>
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

    /// <summary>
    /// Получение тега по Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
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