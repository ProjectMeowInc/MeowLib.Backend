using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Exceptions.DAL;

namespace MeowLib.WebApi.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория книг.
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// Метод создаёт новую сущность книги в базе данных.
    /// </summary>
    /// <param name="entity">Модель для создания.</param>
    /// <returns>Созданную в базе данных сущность.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения.</exception>
    Task<BookEntityModel> CreateAsync(BookEntityModel entity);
    
    /// <summary>
    /// Метод получает книгу по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>Модель книги со всем её полями.</returns>
    Task<BookEntityModel?> GetByIdAsync(int id);
    
    /// <summary>
    /// Метод получает IQueryable всех книг.
    /// </summary>
    /// <returns>IQueryable всех книг.</returns>
    IQueryable<BookEntityModel> GetAll();
    
    /// <summary>
    /// Метод удаляет книгу по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <returns>True - если удаление прошло удачно, false - если книга не была найдена.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<bool> DeleteByIdAsync(int id);
    
    /// <summary>
    /// Метод обновляет информацию о книге.
    /// </summary>
    /// <param name="entity">Модель книги.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="DbSavingException">Возникает в случае ошибки сохранения данных.</exception>
    Task<BookEntityModel> UpdateAsync(BookEntityModel entity);
    
    /// <summary>
    /// Метод обновляет основную информацию о книге по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="updateData">Данные для обновления.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    Task<BookEntityModel> UpdateInfoByIdAsync(int id, UpdateBookEntityModel updateData);
    
    /// <summary>
    /// Метод обновляет автора книги по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="author">Новый автор.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    Task<BookEntityModel> UpdateAuthorByIdAsync(int id, AuthorEntityModel author);
    
    /// <summary>
    /// Метод обновляет теги книги по её Id.
    /// </summary>
    /// <param name="id">Id книги.</param>
    /// <param name="tags">Новый список тегов.</param>
    /// <returns>Обновлённую модель книги.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае если книга не была найдена.</exception>
    /// <exception cref="DbSavingException">Возникате в случае ошибки сохранения данных.</exception>
    Task<BookEntityModel> UpdateTagsByIdAsync(int id, IEnumerable<TagEntityModel> tags);
}