using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Requests.Author;
using MeowLib.WebApi.Abstractions;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace MeowLib.WebApi.Controllers;

[Route("api/authors")]
public class AuthorController : BaseController
{
    private readonly IAuthorService _authorService;
    
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
    public async Task<ActionResult> GetAllAuthors()
    {
        var authors = await _authorService.GetAllAuthors();
        return Json(authors);
    }

    [HttpPost]
    [ProducesResponseType(200, Type = typeof(AuthorDto))]
    public async Task<ActionResult> CreateAuthor([FromBody] CreateAuthorRequest input)
    {
        var author = await _authorService.CreateAuthor(input.Name);
        return Json(author);
    }
}