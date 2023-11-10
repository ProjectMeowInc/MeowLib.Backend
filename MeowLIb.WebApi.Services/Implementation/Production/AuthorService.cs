using AutoMapper;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.Domain.Requests.Author;
using MeowLib.Domain.Result;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с авторами.
/// </summary>
public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="authorRepository">Репозиторий авторов.</param>
    /// <param name="mapper">Автомаппер.</param>
    public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>DTO-модель автора.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    public async Task<Result<AuthorDto>> CreateAuthorAsync(string name)
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
            return Result<AuthorDto>.Fail(new ValidationException(validationErrors));
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
    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAll().Select(a => new AuthorDto
        {
            Id = a.Id,
            Name = a.Name
        }).ToListAsync();
        return authors;
    }

    /// <summary>
    /// Метод обновляет информацию об авторе.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <param name="updateAuthorEntityModel">Данные для обновления.</param>
    /// <returns>Обновлённую модель данных.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если введёные данные некорректны.</exception>
    /// <exception cref="ApiException">Возникает если автор не был найден.</exception>
    public async Task<Result<AuthorDto>> UpdateAuthorAsync(int id, UpdateAuthorEntityModel updateAuthorEntityModel)
    {
        var validationErrors = new List<ValidationErrorModel>();
        
        if (string.IsNullOrEmpty(updateAuthorEntityModel.Name) || updateAuthorEntityModel.Name.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(updateAuthorEntityModel.Name),
                Message = "Имя автора не может быть пустым или короче 6 символов"
            });
        }

        if (validationErrors.Any())
        {
            return Result<AuthorDto>.Fail(new ValidationException(validationErrors));
        }

        try
        {
            return await _authorRepository.UpdateByIdAsync(id, updateAuthorEntityModel);
        }
        catch (EntityNotFoundException entityNotFoundException)
        {
            return Result<AuthorDto>.Fail(entityNotFoundException);
        }
    }

    /// <summary>
    /// Метод удаляет автора.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True - в случае удачного удаления, false - в случае если автор не найден.</returns>
    /// <exception cref="ApiException">Возникает в случае внутренней ошибки.</exception>
    public async Task<Result<bool>> DeleteAuthorAsync(int id)
    {
        try
        {
            return await _authorRepository.DeleteByIdAsync(id);
        }
        catch (DbSavingException)
        {
            return Result<bool>.Fail(new ApiException("Внутреняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Метод получает автора по его Id.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    /// <returns>DTO-модель автора.</returns>
    public async Task<Result<AuthorDto>> GetAuthorByIdAsync(int authorId)
    {
        var foundedAuthor = await _authorRepository.GetByIdAsync(authorId);
        if (foundedAuthor is null)
        {
            return Result<AuthorDto>.Fail(new EntityNotFoundException(nameof(AuthorEntityModel), $"Id = {authorId}"));
        }

        return _mapper.Map<AuthorEntityModel, AuthorDto>(foundedAuthor);
    }

    /// <summary>
    /// Метод получает список авторов подходящих под поисковые параметры.
    /// </summary>
    /// <param name="searchParams">Параметры для поиска.</param>
    /// <returns>Список авторов подходящих под параметры поиска.</returns>
    /// <exception cref="SearchNotFoundException">Возникает если не был найден автор по заданным параметрам поиска.</exception>
    public async Task<Result<IEnumerable<AuthorDto>>> GetAuthorWithParams(GetAuthorWithParamsRequest searchParams)
    {
        var filteredAuthors = _authorRepository.GetAll();

        if (searchParams.Name is not null)
        {
            filteredAuthors = filteredAuthors.Where(a => a.Name.Contains(searchParams.Name));
        }

        if (!filteredAuthors.Any())
        {
            return Result<IEnumerable<AuthorDto>>.Fail(new SearchNotFoundException(nameof(AuthorService)));
        }

        return await filteredAuthors.Select(a => new AuthorDto
        {
            Id = a.Id,
            Name = a.Name
        }).ToListAsync();
    }
}