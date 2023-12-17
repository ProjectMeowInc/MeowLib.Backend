using MeowLib.Services.Interface;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.ProducesResponseTypes;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers.v1;

[ApiController]
[Route("api/v1/images")]
public class FileController(IFileService fileService) : BaseController
{
    [HttpGet("book/{imageName}")]
    [ProducesNotFoundResponseType]
    public async Task<ActionResult> GetBookImage([FromRoute] string imageName)
    {
        var getBookImageResult = await fileService.GetBookImageAsync(imageName);
        if (getBookImageResult.content is null)
        {
            return NotFoundError("Изображение не найдено");
        }

        return File(getBookImageResult.content, getBookImageResult.mimeType);
    }
}