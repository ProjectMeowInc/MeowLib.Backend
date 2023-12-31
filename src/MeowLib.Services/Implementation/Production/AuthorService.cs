using MeowLib.DAL;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Models;
using MeowLib.Domain.Shared.Result;
using MeowLib.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с авторами.
/// </summary>
public class AuthorService(ApplicationDbContext dbContext) : IAuthorService
{
    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>DTO-модель автора.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    public async Task<Result<AuthorEntityModel>> CreateAuthorAsync(string name)
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
            return Result<AuthorEntityModel>.Fail(new ValidationException(validationErrors));
        }

        var entry = await dbContext.Authors.AddAsync(new AuthorEntityModel
        {
            Name = name
        });

        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Метод получает всех авторов.
    /// </summary>
    /// <returns>DTO список авторов.</returns>
    public async Task<IEnumerable<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await dbContext.Authors.Select(a => new AuthorDto
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
    /// <param name="data">Данные для обновления.</param>
    /// <returns>Обновлённую модель данных.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если введёные данные некорректны.</exception>
    public async Task<Result<AuthorEntityModel?>> UpdateAuthorAsync(int id, AuthorDto data)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (string.IsNullOrEmpty(data.Name) || data.Name.Length < 6)
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(data.Name),
                Message = "Имя автора не может быть пустым или короче 6 символов"
            });
        }

        if (validationErrors.Any())
        {
            return Result<AuthorEntityModel?>.Fail(new ValidationException(validationErrors));
        }

        var foundedAuthor = await GetAuthorByIdAsync(id);
        if (foundedAuthor is null)
        {
            return Result<AuthorEntityModel?>.Ok(null);
        }

        foundedAuthor.Name = data.Name;
        dbContext.Authors.Update(foundedAuthor);
        await dbContext.SaveChangesAsync();

        return foundedAuthor;
    }

    /// <summary>
    /// Метод удаляет автора.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True - в случае удачного удаления, false - в случае если автор не найден.</returns>
    public async Task<Result<bool>> DeleteAuthorAsync(int id)
    {
        var foundedAuthor = await GetAuthorByIdAsync(id);
        if (foundedAuthor is null)
        {
            return false;
        }

        dbContext.Authors.Remove(foundedAuthor);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает автора по его Id.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    /// <returns>DTO-модель автора.</returns>
    public Task<AuthorEntityModel?> GetAuthorByIdAsync(int authorId)
    {
        return dbContext.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
    }

    /// <summary>
    /// Метод получает список авторов подходящих под поисковые параметры.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>Список авторов подходящих под параметры поиска.</returns>
    /// <exception cref="SearchNotFoundException">Возникает если не был найден автор по заданным параметрам поиска.</exception>
    public async Task<Result<IEnumerable<AuthorDto>>> GetAuthorWithParams(string? name)
    {
        var filteredAuthors = dbContext.Authors.AsNoTracking();

        if (name is not null)
        {
            filteredAuthors = filteredAuthors.Where(a => a.Name.Contains(name));
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