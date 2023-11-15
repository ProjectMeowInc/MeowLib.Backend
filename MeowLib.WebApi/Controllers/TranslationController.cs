using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Responses.Translation;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/translation")]
public class TranslationController(ITranslationService translationService, ILogger<TranslationController> logger) : BaseController
{
    [HttpGet("{translationId}")]
    [ProducesOkResponseType(typeof(GetAllTranslationChaptersResponse))]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> GetTranslationChapters([FromRoute] int translationId)
    {
        var getTranslationChaptersResult = await translationService.GetTranslationChaptersAsync(translationId);
        if (getTranslationChaptersResult.IsFailure)
        {
            var exception = getTranslationChaptersResult.GetError();
            if (exception is TranslationNotFoundException)
            {
                return Error("Запрашиваемый перевод не найден", 400);
            }
            
            logger.LogError("Ошибка получения глав перевода: {exception}", exception);
        }

        var content = getTranslationChaptersResult.GetResult();
        return Json(new GetAllTranslationChaptersResponse
        {
            Count = content.Count,
            Items = content
        });
    }

    [HttpGet("{translationId}/chapters/{position}")]
    [ProducesOkResponseType(typeof(GetTranslationChapterResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetChapter([FromRoute] int translationId, [FromRoute] int position)
    {
        var foundedChapter = await translationService.GetChapterByTranslationAndPositionAsync(translationId, position);
        if (foundedChapter is null)
        {
            return NotFoundError();
        }

        return Json(new GetTranslationChapterResponse
        {
            Id = foundedChapter.Id,
            Name = foundedChapter.Name,
            Text = foundedChapter.Text,
            Position = foundedChapter.Position,
            ReleaseDate = foundedChapter.ReleaseDate,
        });
    }
}