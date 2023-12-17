using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MeowLib.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с авторами.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<AuthorRepository> _logger;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст база данных.</param>
    /// <param name="logger">Логгер.</param>
    public AuthorRepository(ApplicationDbContext applicationDbContext, ILogger<AuthorRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="createAuthorData">Данный для создания автора.</param>
    /// <returns>DTO-модель автора</returns>
    public async Task<AuthorDto> CreateAsync(CreateAuthorEntityModel createAuthorData)
    {
        var dbResult = await _applicationDbContext.Authors.AddAsync(new AuthorEntityModel
        {
            Name = createAuthorData.Name
        });

        await _applicationDbContext.SaveChangesAsync();

        var createdAuthor = dbResult.Entity;
        return new AuthorDto
        {
            Id = createdAuthor.Id,
            Name = createdAuthor.Name
        };
    }

    /// <summary>
    /// Метод получает модель автора по его Id.
    /// </summary>
    /// <param name="id">Id автора</param>
    /// <returns>Модель автора</returns>
    public async Task<AuthorEntityModel?> GetByIdAsync(int id)
    {
        return await GetAuthorById(id);
    }

    /// <summary>
    /// Метод удаляет автора по Id.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <returns>True - в случае удачного удаления, False - если автор не найден. </returns>
    public async Task<bool> DeleteByIdAsync(int id)
    {
        var foundedAuthor = await GetAuthorById(id);
        if (foundedAuthor is null)
        {
            return false;
        }

        try
        {
            _applicationDbContext.Authors.Remove(foundedAuthor);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException dbUpdateException)
        {
            _logger.LogError("Ошибка удаления автора: {}", dbUpdateException.Message);
            throw;
        }

        return true;
    }

    /// <summary>
    /// Метод получает список всех авторов
    /// </summary>
    /// <returns>Список всех авторов</returns>
    public IQueryable<AuthorEntityModel> GetAll()
    {
        return _applicationDbContext.Authors.AsQueryable();
    }

    /// <summary>
    /// Метод обновляет автора по Id.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <param name="updateAuthorData">Данные для обновления.</param>
    /// <returns>Обновлённую информацию об авторе.</returns>
    public async Task<AuthorDto?> UpdateByIdAsync(int id, UpdateAuthorEntityModel updateAuthorData)
    {
        var foundedAuthor = await GetAuthorById(id);
        if (foundedAuthor is null)
        {
            return null;
        }

        if (updateAuthorData.Name is not null)
        {
            foundedAuthor.Name = updateAuthorData.Name;
        }

        var updatedAuthor = _applicationDbContext.Authors.Update(foundedAuthor).Entity;
        await _applicationDbContext.SaveChangesAsync();

        return new AuthorDto
        {
            Id = updatedAuthor.Id,
            Name = updatedAuthor.Name
        };
    }

    #region Приватные методы

    /// <summary>
    /// Метод получает автора по его Id.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <returns>Модель автора если он был найден, иначе - null</returns>
    private async Task<AuthorEntityModel?> GetAuthorById(int id)
    {
        var foundedAuthor = await _applicationDbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

        return foundedAuthor;
    }

    #endregion
}