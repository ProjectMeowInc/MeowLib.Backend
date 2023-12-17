using MeowLib.DAL.Repository.Interfaces;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL.Repository.Implementation.Production;

/// <summary>
/// Репозиторий комментариев к книге.
/// </summary>
public class BookCommentRepository : IBookCommentRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="applicationDbContext">Контекст ЬД.</param>
    public BookCommentRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    /// <summary>
    /// Метод создаёт новый комментарий в БД.
    /// </summary>
    /// <param name="entity">Модель комментария.</param>
    /// <returns>Комментарий, в случае успеха.</returns>
    public async Task<Result<BookCommentEntityModel>> CreateAsync(BookCommentEntityModel entity)
    {
        var entryEntity = await _applicationDbContext.BookComments.AddAsync(entity);
        await _applicationDbContext.SaveChangesAsync();
        return entryEntity.Entity;
    }

    /// <summary>
    /// Метод возвращает комментарий по его Id.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Комментарий, если он есть.</returns>
    public async Task<BookCommentEntityModel?> GetByIdAsync(int commentId)
    {
        return await _applicationDbContext.BookComments.FirstOrDefaultAsync(c => c.Id == commentId);
    }

    /// <summary>
    /// Метод обновляет комментарий в БД.
    /// </summary>
    /// <param name="entity">Модель для обновления.</param>
    /// <returns>Обновлённый комментарий.</returns>
    public async Task<Result<BookCommentEntityModel>> UpdateAsync(BookCommentEntityModel entity)
    {
        var entryEntity = _applicationDbContext.BookComments.Entry(entity);
        entryEntity.State = EntityState.Modified;

        await _applicationDbContext.SaveChangesAsync();
        return entryEntity.Entity;
    }

    /// <summary>
    /// Метод удаляет комментарий.
    /// </summary>
    /// <param name="entity">Модель для удаления.</param>
    /// <returns>True в случае успеха.</returns>
    public async Task<Result<bool>> DeleteAsync(BookCommentEntityModel entity)
    {
        _applicationDbContext.BookComments.Remove(entity);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Метод возвращает все комментарии.
    /// </summary>
    /// <returns>Комментарии в виде <see cref="IQueryable" /></returns>
    public IQueryable<BookCommentEntityModel> GetAll()
    {
        return _applicationDbContext.BookComments.AsQueryable();
    }
}