using AutoMapper;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Requests.Book;
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
    
    public BookController(IBookService bookService, IBookRepository bookRepository, IMapper mapper)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
        _mapper = mapper;
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

    [HttpPut("{id:int}/info")]
    public async Task<ActionResult> UpdateBookInfo([FromRoute] int id, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookEntityModel = _mapper.Map<UpdateBookInfoRequest, UpdateBookEntityModel>(input);
        
        var updateBookResult = await _bookService.UpdateBookInfoByIdAsync(id, updateBookEntityModel);
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
}