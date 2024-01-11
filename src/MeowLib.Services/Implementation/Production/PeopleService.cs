using MeowLib.DAL;
using MeowLib.Domain.People.Dto;
using MeowLib.Domain.People.Entity;
using MeowLib.Domain.People.Services;
using MeowLib.Domain.Shared.Exceptions;
using MeowLib.Domain.Shared.Models;
using MeowLib.Domain.Shared.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.Services.Implementation.Production;

/// <summary>
/// Сервис для работы с авторами.
/// </summary>
public class PeopleService(ApplicationDbContext dbContext) : IPeopleService
{
    public async Task<Result<PeopleEntityModel>> CreatePeopleAsync(string name)
    {
        var validationErrors = new List<ValidationErrorModel>();

        if (string.IsNullOrEmpty(name))
        {
            validationErrors.Add(new ValidationErrorModel
            {
                PropertyName = nameof(name),
                Message = "Имя человека не может быть пустым"
            });
        }

        if (validationErrors.Any())
        {
            return Result<PeopleEntityModel>.Fail(new ValidationException(validationErrors));
        }

        var entry = await dbContext.Peoples.AddAsync(new PeopleEntityModel
        {
            Name = name,
            BooksPeople = []
        });

        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    /// <summary>
    /// Метод получает всех людей.
    /// </summary>
    /// <returns>DTO список людей.</returns>
    public async Task<IEnumerable<PeopleDto>> GetAllPeoplesAsync()
    {
        var peoples = await dbContext.Peoples.Select(a => new PeopleDto
        {
            Id = a.Id,
            Name = a.Name
        }).ToListAsync();
        return peoples;
    }

    public async Task<List<PeopleEntityModel>> GetAllPeoplesWithPageAsync(int perPageCount, int page)
    {
        return await dbContext.Peoples
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Skip(page * perPageCount)
            .Take(perPageCount)
            .ToListAsync();
    }

    public async Task<Result<PeopleEntityModel?>> UpdatePeopleAsync(int id, PeopleDto data)
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
            return Result<PeopleEntityModel?>.Fail(new ValidationException(validationErrors));
        }

        var foundedAuthor = await GetPeopleByIdAsync(id);
        if (foundedAuthor is null)
        {
            return Result<PeopleEntityModel?>.Ok(null);
        }

        foundedAuthor.Name = data.Name;
        dbContext.Peoples.Update(foundedAuthor);
        await dbContext.SaveChangesAsync();

        return foundedAuthor;
    }

    /// <summary>
    /// Метод удаляет человека.
    /// </summary>
    /// <param name="peopleId">Id человека.</param>
    /// <returns>True - в случае удачного удаления, false - в случае если человек не найден.</returns>
    public async Task<Result<bool>> DeletePeopleAsync(int peopleId)
    {
        var foundedAuthor = await GetPeopleByIdAsync(peopleId);
        if (foundedAuthor is null)
        {
            return false;
        }

        dbContext.Peoples.Remove(foundedAuthor);
        await dbContext.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Метод получает человека по его Id.
    /// </summary>
    /// <param name="peopleId">Id автора.</param>
    /// <returns>DTO-модель автора.</returns>
    public Task<PeopleEntityModel?> GetPeopleByIdAsync(int peopleId)
    {
        return dbContext.Peoples
            .Include(p => p.BooksPeople)
            .ThenInclude(bp => bp.Book)
            .ThenInclude(bp => bp.Image)
            .FirstOrDefaultAsync(a => a.Id == peopleId);
    }

    /// <summary>
    /// Метод получает список людей подходящих под поисковые параметры.
    /// </summary>
    /// <param name="name">Имя человека.</param>
    /// <returns>Список людей подходящих под параметры поиска.</returns>
    /// <exception cref="SearchNotFoundException">Возникает если не был найден человек по заданным параметрам поиска.</exception>
    public async Task<Result<IEnumerable<PeopleDto>>> GetPeopleWithParams(string? name)
    {
        var filteredAuthors = dbContext.Peoples.AsNoTracking();

        if (name is not null)
        {
            filteredAuthors = filteredAuthors.Where(a => a.Name.Contains(name));
        }

        if (!filteredAuthors.Any())
        {
            return Result<IEnumerable<PeopleDto>>.Fail(new SearchNotFoundException(nameof(PeopleService)));
        }

        return await filteredAuthors.Select(a => new PeopleDto
        {
            Id = a.Id,
            Name = a.Name
        }).ToListAsync();
    }
}