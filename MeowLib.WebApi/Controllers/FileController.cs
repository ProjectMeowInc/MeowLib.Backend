using MeowLib.WebApi.Abstractions;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[ApiController]
[Route("api/images")]
public class FileController : BaseController
{
    private readonly IUploadFileService _uploadFileService;
    
    public FileController(IUploadFileService uploadFileService)
    {
        _uploadFileService = uploadFileService;
    }

    [HttpGet("book/{imageName}")]
    public async Task<ActionResult> GetBookImage([FromRoute] string imageName)
    {
        var getBookImageResult = await _uploadFileService.GetBookImageAsync(imageName);
        if (getBookImageResult.content is null)
        {
            return NotFoundError("Изображение не найдено");
        }
        
        return File(getBookImageResult.content, getBookImageResult.mimeType);
    }
}