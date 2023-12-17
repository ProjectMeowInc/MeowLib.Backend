using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.Result;

namespace MeowLib.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория комментариев
/// </summary>
public interface IBookCommentRepository
{
    /// <summary>
    /// Метод создаёт новый комментарий в БД.
    /// </summary>
    /// <param name="entity">Модель комментария.</param>
    /// <returns>Комментарий, в случае успеха.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<BookCommentEntityModel>> CreateAsync(BookCommentEntityModel entity);

    /// <summary>
    /// Метод возвращает комментарий по его Id.
    /// </summary>
    /// <param name="commentId">Id комментария.</param>
    /// <returns>Комментарий, если он есть.</returns>
    Task<BookCommentEntityModel?> GetByIdAsync(int commentId);

    /// <summary>
    /// Метод обновляет комментарий в БД.
    /// </summary>
    /// <param name="entity">Модель для обновления.</param>
    /// <returns>Обновлённый комментарий.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<BookCommentEntityModel>> UpdateAsync(BookCommentEntityModel entity);

    /// <summary>
    /// Метод удаляет комментарий.
    /// </summary>
    /// <param name="entity">Модель для удаления.</param>
    /// <returns>True в случае успеха.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<bool>> DeleteAsync(BookCommentEntityModel entity);

    /// <summary>
    /// Метод возвращает все комментарии.
    /// </summary>
    /// <returns>Комментарии в виде <see cref="IQueryable" /></returns>
    public IQueryable<BookCommentEntityModel> GetAll();
}