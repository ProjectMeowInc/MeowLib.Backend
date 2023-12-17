using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[ApiController]
[Route("api/v1/images")]
public class FileController : BaseController
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("book/{imageName}")]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookImage([FromRoute] string imageName)
    {
        var getBookImageResult = await _fileService.GetBookImageAsync(imageName);
        if (getBookImageResult.content is null)
        {
            return NotFoundError("Изображение не найдено");
        }

        return File(getBookImageResult.content, getBookImageResult.mimeType);
    }
}