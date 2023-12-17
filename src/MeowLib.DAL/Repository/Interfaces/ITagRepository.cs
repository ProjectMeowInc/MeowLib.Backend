using MeowLib.Domain.DbModels.TagEntity;

namespace MeowLib.DAL.Repository.Interfaces;

/// <summary>
/// Абстракция репозитория для работы с тегами.
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Метод возвращает все теги как IQueryable.
    /// </summary>
    /// <returns>IQuerable всех тегов.</returns>
    IQueryable<TagEntityModel> GetAll();

    /// <summary>
    /// Метод создаёт новый тег.
    /// </summary>
    /// <param name="createTagData">Данные для создания тега.</param>
    /// <returns>Созданные тег.</returns>
    /// <exception cref="DbSavingException">Возникает в случае, если произошла ошибка сохранения данных.</exception>
    Task<TagEntityModel> CreateAsync(CreateTagEntityModel createTagData);

    /// <summary>
    /// Метод удаляет тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>True - в случае удачного удаления, false - если тег не был найден.</returns>
    /// <exception cref="DbSavingException">Возникает в случае, если произошла ошибка сохранения данных.</exception>
    Task<bool> DeleteByIdAsync(int id);

    /// <summary>
    /// Метод обновляет информацию о теге.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <param name="updateTagData">Информация на обновление.</param>
    /// <returns>Обновлённая информация о теге.</returns>
    /// <exception cref="EntityNotFoundException">Возникает в случае, если тег не был найден.</exception>
    /// <exception cref="DbSavingException">Возникает в случае если произошла ошибка сохранения данных.</exception>
    Task<TagEntityModel> UpdateAsync(int id, UpdateTagEntityModel updateTagData);

    /// <summary>
    /// Метод получает тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>Тег или null в случае если тег не бы найден.</returns>
    Task<TagEntityModel?> GetByIdAsync(int id);
}