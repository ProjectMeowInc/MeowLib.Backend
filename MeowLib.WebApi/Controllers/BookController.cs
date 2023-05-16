using AutoMapper;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Requests.Book;
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
    public async Task<ActionResult> GetAllBooks()
    {
        var books = await _bookRepository.GetAll().Select(b => new
        {
            b.Id,
            b.Description,
            b.Name,
            AuthorName = b.Author.Name
        }).ToListAsync();

        return Json(books);
    }

    [HttpPost, Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    public async Task<ActionResult> CreateBook([FromBody] CreateBookRequest input)
    {
        var createBookEntityModel = _mapper.Map<CreateBookRequest, CreateBookEntityModel>(input);
        var createdBook = await _bookService.CreateBookAsync(createBookEntityModel);
        return Json(createdBook);
    }

    [HttpDelete("{id:int}"), Authorization(RequiredRoles = new [] { UserRolesEnum.Admin, UserRolesEnum.Editor })]
    public async Task<ActionResult> DeleteBook([FromRoute] int id)
    {
        var bookDeleted = await _bookService.DeleteBookByIdAsync(id);
        if (!bookDeleted)
        {
            return Error($"Книга с Id = {id} не найдена");
        }

        return Ok();
    }

    [HttpPut("{id:int}/info")]
    public async Task<ActionResult> UpdateBookInfo([FromRoute] int id, [FromBody] UpdateBookInfoRequest input)
    {
        var updateBookEntityModel = _mapper.Map<UpdateBookInfoRequest, UpdateBookEntityModel>(input);
        
        var updatedBook = await _bookService.UpdateBookInfoByIdAsync(id, updateBookEntityModel);
        if (updatedBook is null)
        {
            return Error($"Книга с Id = {id} не найдена");
        }

        return Json(updatedBook);
    }
}