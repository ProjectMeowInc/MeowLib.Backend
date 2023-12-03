using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Enums;
using MeowLib.Domain.Exceptions.DAL;
using MeowLib.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий книг.
/// </summary>
public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных.</param>
    public BookRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод создаёт новую сущность книги в базе данных.
    /// </summary>
    /// <param name="entity">Модель для создания.</param>
    /// <returns>Созданную в базе данных сущность.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения.</exception>
    public async Task<BookEntityModel> CreateAsync(BookEntityModel entity)
    {
        var result = await _applicationDbContext.Books.AddAsync(entity);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return result.Entity;
        }
        catch (DbUpdateException)
        {
            throw new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Create);
        }
    }

    /// <summary>
    /// Метод получает книгу по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>Модель книги со всем её полями.</returns>
    public async Task<BookEntityModel?> GetByIdAsync(int id)
    {
        return await _applicationDbContext.Books
            .Include(b => b.Author)
            .Include(b => b.Tags)
            .Include(b => b.Translations)
            .ThenInclude(t => t.Team)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// Метод получает IQueryable всех книг.
    /// </summary>
    /// <returns>IQueryable всех книг.</returns>
    public IQueryable<BookEntityModel> GetAll()
    {
        return _applicationDbContext.Books.AsQueryable();
    }

    /// <summary>
    /// Метод удаляет книгу по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>True - если удаление прошло удачно, false - если книга не была найдена.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<bool> DeleteByIdAsync(int id)
    {
        var foundedBook = await GetByIdAsync(id);
        if (foundedBook is null)
        {
            return false;
        }

        try
        {
            _applicationDbContext.Books.Remove(foundedBook);
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Delete);
        }

        return true;
    }

    /// <summary>
    /// Метод обновляет информацию о книге.
    /// </summary>
    /// <param name="entity">Модель книги.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    public async Task<Result<BookEntityModel>> UpdateAsync(BookEntityModel entity)
    {
        try
        {
            var entryEntity = _applicationDbContext.Entry(entity);
            entryEntity.State = EntityState.Modified;
            await _applicationDbContext.SaveChangesAsync();
            return entryEntity.Entity;
        }
        catch (DbUpdateException)
        {
            var dbSavingException = new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Update);
            return Result<BookEntityModel>.Fail(dbSavingException);
        }
    }

    /// <summary>
    /// Метод обновляет основную информацию о книге по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="updateData">Данные для обновления.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    public async Task<BookEntityModel> UpdateInfoByIdAsync(int id, UpdateBookEntityModel updateData)
    {
        var foundedBook = await GetByIdAsync(id);
        if (foundedBook is null)
        {
            throw new EntityNotFoundException(nameof(BookEntityModel), $"Id = {id}");
        }

        if (updateData.Name is not null)
        {
            foundedBook.Name = updateData.Name;
        }

        if (updateData.Description is not null)
        {
            foundedBook.Description = updateData.Description;
        }

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Update);
        }

        return foundedBook;
    }

    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="author">Новый автор.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    public async Task<BookEntityModel> UpdateAuthorByIdAsync(int id, AuthorEntityModel author)
    {
        var foundedBook = await GetByIdAsync(id);
        if (foundedBook is null)
        {
            throw new EntityNotFoundException(nameof(BookEntityModel), $"Id = {id}");
        }

        foundedBook.Author = author;
        _applicationDbContext.Books.Update(foundedBook);
        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return foundedBook;
        }
        catch (DbUpdateException)
        {
            throw new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Update);
        }
    }

    /// <summary>
    /// Метод обновляет теги книги по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="tags">Новый список тегов.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    public async Task<BookEntityModel> UpdateTagsByIdAsync(int id, IEnumerable<TagEntityModel> tags)
    {
        var foundedBook = await GetByIdAsync(id);
        if (foundedBook is null)
        {
            throw new EntityNotFoundException(nameof(BookEntityModel), $"Id = {id}");
        }

        foundedBook.Tags = tags;
        _applicationDbContext.Books.Update(foundedBook);
        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return foundedBook;
        }
        catch (DbUpdateException)
        {
            throw new DbSavingException(nameof(BookEntityModel), DbSavingTypesEnum.Update);
        }
    }
}