using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.Tag;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий для работы с тегами.
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных</param>
    public TagRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод возвращает все теги как IQueryable.
    /// </summary>
    /// <returns>IQuerable всех тегов.</returns>
    public IQueryable<TagEntityModel> GetAll()
    {
        return _applicationDbContext.Tags.AsQueryable();
    }

    /// <summary>
    /// Метод создаёт новый тег.
    /// </summary>
    /// <param name="createTagData">Данные для создания тега.</param>
    /// <returns>Созданные тег.</returns>
    public async Task<TagEntityModel> CreateAsync(CreateTagEntityModel createTagData)
    {
        var newTag = new TagEntityModel
        {
            Name = createTagData.Name,
            Description = createTagData.Description ?? string.Empty,
            Books = new List<BookEntityModel>()
        };

        var dbResult = await _applicationDbContext.Tags.AddAsync(newTag);
        await _applicationDbContext.SaveChangesAsync();


        return dbResult.Entity;
    }

    /// <summary>
    /// Метод удаляет тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>True - в случае удачного удаления, false - если тег не был найден.</returns>
    /// <exception cref="DbSavingException">Возникает в случае, если произошла ошибка сохранения данных.</exception>
    public async Task<bool> DeleteByIdAsync(int id)
    {
        var foundedTag = await GetByIdAsync(id);
        if (foundedTag is null)
        {
            return false;
        }

        _applicationDbContext.Tags.Remove(foundedTag);
        await _applicationDbContext.SaveChangesAsync();
        
        return true;
    }

    /// <summary>
    /// Метод обновляет информацию о теге.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <param name="updateTagData">Информация на обновление.</param>
    /// <returns>Обновлённая информация о теге.</returns>
    /// <exception cref="TagNotFoundException">Возникает в случае, если тег не был найден.</exception>
    public async Task<TagEntityModel> UpdateAsync(int id, UpdateTagEntityModel updateTagData)
    {
        var foundedTag = await GetByIdAsync(id);
        if (foundedTag is null)
        {
            throw new TagNotFoundException(id);
        }

        if (updateTagData.Name is not null)
        {
            foundedTag.Name = updateTagData.Name;
        }

        if (updateTagData.Description is not null)
        {
            foundedTag.Description = updateTagData.Description;
        }

        _applicationDbContext.Tags.Update(foundedTag);

        await _applicationDbContext.SaveChangesAsync();

        return foundedTag;
    }

    /// <summary>
    /// Метод получает тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>Тег или null в случае если тег не бы найден.</returns>
    public async Task<TagEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
    }
}