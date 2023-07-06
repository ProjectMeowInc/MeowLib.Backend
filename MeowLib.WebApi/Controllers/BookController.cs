using AutoMapper;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.Dto.Book;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Book;
using MeowLib.Domain.Requests.Chapter;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.Book;
using MeowLib.Domain.Responses.Chapter;
using MeowLib.WebApi.Abstractions;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLib.WebApi.Filters;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.Controllers;

[Route("api/books")]
public class BookController : BaseController
{
    private readonly IBookService _bookService;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IChapterService _chapterService;
    private readonly IFileService _fileService;
    
    public BookController(IBookService bookService, IBookRepository bookRepository, IMapper mapper,
        IChapterService chapterService, IFileService fileService)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
        _mapper = mapper;
        _chapterService = chapterService;
        _fileService = fileService;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(GetAllBooksResponse))]
    public async Task<ActionResult> GetAllBooks()
    {
        var response = new GetAllBooksResponse
        {
            Items = await _bookRepository.GetAll().Select(book => new BookDto
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description
            }).ToListAsync(),
        };

        return Json(response);
    }

    [HttpPost, Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesResponseType(200, Type = typeof(BookEntityModel))]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest input)
    {
        var createBookEntityModel = _mapper.Map<CreateBookRequest, CreateBookEntityModel>(input);
        
        var createBookResult = await _bookService.CreateBookAsync(createBookEntityModel);

        return createBookResult.Match<ActionResult>(createdBook => Json(createdBook), exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            return ServerError();
        });
    }

    [HttpDelete("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesResponseType(200)]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> DeleteBook([FromRoute] int id)
    {
        var deleteBookResult = await _bookService.DeleteBookByIdAsync(id);

        return deleteBookResult.Match<ActionResult>(ok =>
        {
            if (!ok)
            {
                return NotFoundError();
            }

            return Ok();
        }, _ => ServerError());
    }

    [HttpPut("{bookId:int}/info"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesResponseType(200)]
    [ProducesResponseType(403, Type = typeof(ValidationErrorResponse))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateBookInfo([FromRoute] int bookId, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookEntityModel = _mapper.Map<UpdateBookInfoRequest, UpdateBookEntityModel>(input);
        
        var updateBookResult = await _bookService.UpdateBookInfoByIdAsync(bookId, updateBookEntityModel);
        return updateBookResult.Match<ActionResult>(updatedBook =>
        {
            if (updatedBook is null)
            {
                return Error($"Книга с Id = {bookId} не найдена");
            }
            
            return Empty();
        }, exception =>
        {
            if (exception is ValidationException validationException)
            {
                return validationException.ToResponse();
            }

            return ServerError();
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetBook([FromRoute] int id)
    {
        var foundedBook = await _bookService.GetBookByIdAsync(id);
        if (foundedBook is null)
        {
            return NotFoundError();
        }

        var getBookResponse = _mapper.Map<BookEntityModel, GetBookResponse>(foundedBook);
        return Json(getBookResponse);
    }

    [HttpPost("{bookId:int}/chapters"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> CreateChapter([FromRoute] int bookId, [FromBody] CreateChapterRequest input)
    {
        var createChapterResult = await _chapterService.CreateChapterAsync(name: input.Name, text: input.Text, bookId: bookId);
        return createChapterResult.Match<ActionResult>(_ => Empty(), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error($"Книга с Id = {bookId} не найдена", 400);
            }

            return ServerError();
        });
    }

    [HttpPut("{bookId:int}/chapters/{chapterId:int}/text")]
    [ProducesResponseType(200, Type = typeof(ChapterEntityModel))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateChapterText([FromRoute] int chapterId, 
        [FromBody] UpdateChapterRequest input)
    {
        var updateChapterTextResult = await _chapterService.UpdateChapterTextAsync(chapterId, input.Text);
        return updateChapterTextResult.Match<ActionResult>(updatedChapter => Json(updatedChapter), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error($"Глава с Id = {chapterId} не найдена", 400);
            }

            return ServerError();
        });
    }

    [HttpGet("{bookId:int}/chapters")]
    [ProducesResponseType(200, Type = typeof(GetAllBookChaptersResponse))]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetBookChapterList([FromRoute] int bookId)
    {
        var getChaptersResult = await _chapterService.GetAllBookChapters(bookId);

        return getChaptersResult.Match<ActionResult>(chapters => Json(new GetAllBookChaptersResponse
        {
            Items = chapters
        }), exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error($"Книга с Id = {bookId} не найдена", 400);
            }

            return ServerError();
        });
    }

    [HttpDelete("{bookId:int}/chapters/{chapterId:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> DeleteBookChapter([FromRoute] int chapterId)
    {
        var deleteChapterResult = await _chapterService.DeleteChapterAsync(chapterId);

        return deleteChapterResult.Match<ActionResult>(exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error($"Глава с Id = {chapterId} не найдена", 400);
            }

            return ServerError();
        }, () => Empty());
    }

    [HttpGet("{bookId:int}/chapters/{chapterId:int}")]
    [ProducesResponseType(200, Type = typeof(GetBookChapterResponse))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> GetBookChapter([FromRoute] int chapterId)
    {
        var foundedChapter = await _chapterService.GetChapterByIdAsync(chapterId);
        if (foundedChapter is null)
        {
            return NotFoundError();
        }

        var mappedResponse = _mapper.Map<ChapterEntityModel, GetBookChapterResponse>(foundedChapter);
        return Json(mappedResponse);
    }

    [HttpPut("{bookId:int}/author/{authorId:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    [ProducesResponseType(200)]
    [ProducesResponseType(400, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(404, Type = typeof(BaseErrorResponse))]
    [ProducesResponseType(500, Type = typeof(BaseErrorResponse))]
    public async Task<ActionResult> UpdateBookAuthor([FromRoute] int bookId, [FromRoute] int authorId)
    {
        var updateBookResult = await _bookService.UpdateBookAuthorAsync(bookId, authorId);
        return updateBookResult.Match<ActionResult>(updatedBook =>
        {
            if (updatedBook is null)
            {
                return NotFoundError();
            }

            return Empty(200);
        }, exception =>
        {
            if (exception is EntityNotFoundException)
            {
                return Error($"Автор с Id = {authorId} не найден", 400);
            }

            return ServerError();
        });
    }

    [HttpPut("{bookId:int}/tags"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    public async Task<ActionResult> UpdateBookTags([FromRoute] int bookId, [FromBody] UpdateBookTagsRequest input)
    {
        var updateBookResult = await _bookService.UpdateBookTags(bookId, input.Tags);
        return updateBookResult.Match<ActionResult>(updatedBook =>
        {
            if (updatedBook is null)
            {
                return NotFoundError();
            }

            return Empty();
        }, _ => ServerError());
    }
    
    [HttpPut("{bookId:int}/image"), Authorization(RequiredRoles = new [] { UserRolesEnum.Editor, UserRolesEnum.Admin })]
    public async Task<ActionResult> UpdateBookImage(IFormFile image)
    {
        var uploadImageResult = await _fileService.UploadBookImageAsync(image);
        return uploadImageResult.Match<ActionResult>(Ok, exception =>
        {
            if (exception is FileHasIncorrectExtensionException fileHasIncorrectExtensionException)
            {
                return Error(
                    $"Файл имеет некорректное расширение: {fileHasIncorrectExtensionException.CurrentExtension}");
            }
            
            return ServerError();
        });
    }
}