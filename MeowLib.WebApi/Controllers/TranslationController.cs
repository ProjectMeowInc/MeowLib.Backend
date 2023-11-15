using MeowLib.Domain.Exceptions.Translation;
using MeowLib.Domain.Requests.Translation;
using MeowLib.Domain.Responses.Translation;
using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/translation")]
public class TranslationController(ITranslationService translationService, ITeamService teamService, 
    IBookService bookService, ILogger<TranslationController> logger) : BaseController
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
}