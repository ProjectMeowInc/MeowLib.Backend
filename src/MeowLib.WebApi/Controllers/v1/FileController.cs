using MeowLib.Domain.File.Exceptions;
using MeowLib.Domain.File.Services;
using MeowLib.Domain.User.Enums;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.Filters;
using MeowLib.WebApi.Models.Responses.v1.File;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

/// <summary>
/// Контроллер файлов.
/// </summary>
/// <param name="fileService">Сервис файлов.</param>
[ApiController]
[Route("api/v1/images")]
public class FileController(IFileService fileService, ILogger<FileController> logger) : BaseController
{
    /// <summary>
    /// Получение изображения по его имени.
    /// </summary>
    /// <param name="imageName">Название изображение.</param>
    [HttpGet("{imageName}")]
    public async Task<IActionResult> GetImageByName([FromRoute] string imageName)
    {
        var result = await fileService.GetFileByNameAsync(imageName);
        if (result is null)
        {
            return NotFoundError("Запрашиваемый файл не найден");
        }

        var (data, contentType) = result.Value;

        return File(data, contentType);
    }

    /// <summary>
    /// Метод загружает файл.
    /// </summary>
    /// <param name="file">Файл для загрузки.</param>
    [HttpPost("upload")]
    [Authorization(RequiredRoles = new[] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesOkResponseType(typeof(UploadFileResponse))]
    [ProducesUserErrorResponseType]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        var result = await fileService.UploadFileAsync(file);
        if (result.IsFailure)
        {
            var exception = result.GetError();
            if (exception is FileHasIncorrectExtensionException)
            {
                return Error("У файла некоректное расширение", 400);
            }

            if (exception is FileIsTooBigException)
            {
                return Error("Файл слишком большой", 400);
            }

            logger.LogError("Неизвестная ошибка загрузки файла: {exception}", exception);
            return ServerError();
        }

        return Ok(new UploadFileResponse
        {
            CreatedId = result.GetResult().Id
        });
    }
}