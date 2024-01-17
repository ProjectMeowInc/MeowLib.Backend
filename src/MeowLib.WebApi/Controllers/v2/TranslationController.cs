using MeowLib.Domain.Chapter.Exceptions;
using MeowLib.Domain.Translation.Exceptions;
using MeowLib.Domain.Translation.Services;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v2.Translation;
using MeowLib.WebApi.Models.Responses.v2.Translation;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v2;

[Route("api/v2/translation")]
public class TranslationController(
    ITranslationService translationService,
    ILogger<TranslationController> logger) : BaseController
{
    /// <summary>
    /// Загрузка новой главы.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="payload">Данные для создания главы.</param>
    [HttpPost("{translationId}/upload-chapter")]
    [Authorization]
    [RequiredTeam]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> UploadChapter([FromRoute] int translationId,
        [FromBody] UploadChapterRequest payload)
    {
        var foundedTranslation = await translationService.GetTranslationByIdAsync(translationId);
        if (foundedTranslation is null)
        {
            return Error("Запрашиваемый перевод не найден", 400);
        }

        var userTeams = GetUserTeams();
        if (userTeams.All(t => t.Id != foundedTranslation.Team.Id))
        {
            return Error("Вы не состоите в данной комманде", 400);
        }

        var addChapterResult = await translationService.AddChapterAsync(translationId, payload.Name, payload.Text,
            payload.Position, payload.Volume);

        if (addChapterResult.IsFailure)
        {
            var exception = addChapterResult.GetError();
            if (exception is TranslationNotFoundException)
            {
                return Error("Запрашиваемый перевод не найден", 400);
            }

            if (exception is ChapterPositionAlreadyTaken)
            {
                return Error("Позиция для главы уже занята", 400);
            }

            logger.LogError("Неизвестная ошибка загрузки главы: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }

    /// <summary>
    /// Получение списка томов перевода.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    [HttpGet("{translationId}/volume")]
    [ProducesOkResponseType(typeof(GetTranslationVolumesResponse))]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> GetTranslationVolumes([FromRoute] int translationId)
    {
        var getTranslationChaptersResult = await translationService.GetTranslationChaptersAsync(translationId);
        if (getTranslationChaptersResult.IsFailure)
        {
            var exception = getTranslationChaptersResult.GetError();
            if (exception is TranslationNotFoundException)
            {
                return NotFoundError();
            }

            logger.LogError("Неизвестная ошибка получения глав перевода: {exception}", exception);
            return ServerError();
        }

        var volumes = getTranslationChaptersResult.GetResult()
            .GroupBy(c => c.Volume)
            .Select(group => new VolumeModel
            {
                Number = group.Key,
                Chapters = group.Select(chap => new ChapterModel
                    {
                        Id = chap.Id,
                        Name = chap.Name,
                        ReleaseDate = chap.ReleaseDate,
                        Position = chap.Position
                    })
                    .ToList()
            });
        return Ok(new GetTranslationVolumesResponse
        {
            Volumes = volumes
        });
    }
}