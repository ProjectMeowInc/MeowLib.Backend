using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="authorRepository">Репозиторий авторов.</param>
    public AuthorService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>DTO-модель автора.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    public async Task<AuthorDto> CreateAuthor(string name)
    {
        var validationErrors = new List<ValidationErrorModel>();
        
        if (string.IsNullOrEmpty(name))
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(name),
                Message = "Имя автора не может быть пустым"
            });
        }

        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);
        }

        var authorData = new CreateAuthorEntityModel
        {
            Name = name
        };

        var createAuthor = await _authorRepository.CreateAsync(authorData);
        return createAuthor;
    }

    /// <summary>
    /// Метод получает всех авторов.
    /// </summary>
    /// <returns>DTO список авторов.</returns>
    public async Task<IEnumerable<AuthorDto>> GetAllAuthors()
    {
        var authors = await _authorRepository.GetAll();
        return authors;
    }
}