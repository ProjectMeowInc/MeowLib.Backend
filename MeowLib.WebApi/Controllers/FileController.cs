﻿using MeowLib.WebApi.Abstractions;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[ApiController]
[Route("api/images")]
public class FileController : BaseController
{
    private readonly IFileService _fileService;
    
    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet("book/{imageName}")]
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