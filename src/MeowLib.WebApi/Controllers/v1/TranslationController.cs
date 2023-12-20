using MeowLib.Domain.Exceptions.Chapter;
using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Requests.v1.Translation;
using MeowLib.WebApi.Models.Responses.v1.Translation;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер переводов
/// </summary>
/// <param name="translationService">Сервис переводов.</param>
/// <param name="teamService">Сервис комманд.</param>
/// <param name="bookService">Сервис книг.</param>
/// <param name="logger">Логгер.</param>
[Route("api/v1/translation")]
public class TranslationController(
    ITranslationService translationService,
    ITeamService teamService,
    IBookService bookService,
    ILogger<TranslationController> logger) : BaseController
{
    /// <summary>
    /// Получение глав перевода.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
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
            return ServerError();
        }

        var content = getTranslationChaptersResult.GetResult();
        return Json(new GetAllTranslationChaptersResponse
        {
            Count = content.Count,
            Items = content
        });
    }

    /// <summary>
    /// Получение главы в переводе.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="position">Позиция в переводе.</param>
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
            ReleaseDate = foundedChapter.ReleaseDate
        });
    }

    /// <summary>
    /// Создание нового перевода.
    /// </summary>
    /// <param name="payload">Данные для создания.</param>
    [HttpPost]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> CreateTranslation([FromBody] CreateTranslationRequest payload)
    {
        var requestUserData = await GetUserDataAsync();

        var foundedTeam = await teamService.GetTeamByIdAsync(payload.TeamId);
        if (foundedTeam is null)
        {
            logger.LogWarning("Коммана с Id = {teamId} для создания перевода не найдена", payload.TeamId);
            return NotFoundError();
        }

        var isUserHasTeamAdminAccess = await teamService.CheckUserIsTeamAdminAsync(payload.TeamId, requestUserData.Id);
        if (!isUserHasTeamAdminAccess)
        {
            logger.LogWarning("Пользователь с Id = {userId} без админ-привелегий в комманде попытался добавить перевод",
                requestUserData.Id);
            return Error("У вас нету доступа к добавлению перевода от имени комманды", 400);
        }

        var foundedBook = await bookService.GetBookByIdAsync(payload.BookId);
        if (foundedBook is null)
        {
            logger.LogWarning("Книга с Id = {bookId} для создания перевода не найдена", payload.BookId);
            return NotFoundError();
        }

        var createTranslationResult = await translationService.CreateTranslationAsync(foundedBook, foundedTeam);
        if (createTranslationResult.IsFailure)
        {
            var exception = createTranslationResult.GetError();
            if (exception is TeamAlreadyTranslateBookException)
            {
                logger.LogWarning("Комманда с Id = {teamId} уже занимается переводом книги с Id = {bookId}",
                    payload.TeamId, payload.BookId);
                return Error("Комманда уже занимается переводом данной книги", 400);
            }
        }

        return Ok();
    }

    /// <summary>
    /// Добавление главы в перевод.
    /// </summary>
    /// <param name="translationId">Id перевода.</param>
    /// <param name="payload">Данные для добавления главы.</param>
    [HttpPost("{translationId}/upload-chapter")]
    [Authorization]
    [ProducesOkResponseType]
    [ProducesUserErrorResponseType]
    [ProducesNotFoundResponseType]
    public async Task<IActionResult> AddChapterToTranslation([FromRoute] int translationId,
        [FromBody] AddChapterToTranslationRequest payload)
    {
        var requestUserData = await GetUserDataAsync();

        var foundedTranslation = await translationService.GetTranslationByIdAsync(translationId);
        if (foundedTranslation is null)
        {
            logger.LogWarning("Пользователь с Id = {userId} пытался добавить главу к несуществующему переводу",
                requestUserData.Id);
            return NotFoundError();
        }

        var addChapterResult =
            await translationService.AddChapterAsync(translationId, payload.Name, payload.Text, payload.Position);
        if (addChapterResult.IsFailure)
        {
            var exception = addChapterResult.GetError();
            if (exception is ChapterPositionAlreadyTaken)
            {
                logger.LogWarning("При добавлении главы позиция была занята: {position}", payload.Position);
                return Error("Заданная позиция уже занята", 400);
            }

            logger.LogError("При добавлении главы произошла ошибка: {exception}", exception);
            return ServerError();
        }

        return Ok();
    }
}