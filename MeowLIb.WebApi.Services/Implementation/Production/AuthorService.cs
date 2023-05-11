using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.WebApi.DAL.Repository.Interfaces;
using MeowLIb.WebApi.Services.Interface;

namespace MeowLIb.WebApi.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с авторами.
/// </summary>
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
    public async Task<AuthorDto> CreateAuthorAsync(string name)
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
    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAll();
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
    public async Task<AuthorDto> UpdateAuthorAsync(int id, UpdateAuthorEntityModel updateAuthorEntityModel)
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
            throw new ValidationException(validationErrors);
        }

        try
        {
            return await _authorRepository.UpdateByIdAsync(id, updateAuthorEntityModel);
        }
        catch (EntityNotFoundException)
        {
            throw new ApiException($"Автор с Id {id} не найден");
        }
    }

    /// <summary>
    /// Метод удаляет автора.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True - в случае удачного удаления, false - в случае если автор не найден.</returns>
    /// <exception cref="ApiException">Возникает в случае внутренней ошибки.</exception>
    public async Task<bool> DeleteAuthorAsync(int id)
    {
        try
        {
            return await _authorRepository.DeleteByIdAsync(id);
        }
        catch (DbSavingException)
        {
            throw new ApiException("Внутреняя ошибка сервера");
        }
    }
}