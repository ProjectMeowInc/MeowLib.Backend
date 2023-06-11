using AutoMapper;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Book;
using MeowLib.Domain.Requests.Chapter;
using MeowLib.Domain.Responses;
using MeowLib.Domain.Responses.Book;
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
    
    public BookController(IBookService bookService, IBookRepository bookRepository, IMapper mapper, IChapterService chapterService)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
        _mapper = mapper;
        _chapterService = chapterService;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(GetAllBooksResponse))]
    public async Task<ActionResult> GetAllBooks()
    {
        var response = new GetAllBooksResponse
        {
            Items = await _bookRepository.GetAll().Select(book => book).ToListAsync(),
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

    [HttpPut("{bookId:int}/info")]
    [ProducesResponseType(200, Type = typeof(BookEntityModel))]
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
                return NotFoundError();
            }

            return Json(updatedBook);
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
}