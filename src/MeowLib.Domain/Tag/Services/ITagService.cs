using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Result;
using MeowLib.Domain.Tag.Dto;
using MeowLib.Domain.Tag.Entity;

namespace MeowLib.Domain.Tag.Services;

/// <summary>
/// Абстракция сервси для работы с тегами.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Метод создаёт новый тег.
    /// </summary>
    /// <param name="name">Название тега.</param>
    /// <param name="description">Описание тега.</param>
    /// <returns>Информацию о созданном теге.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<Result<TagEntityModel>> CreateTagAsync(string name, string? description);

    /// <summary>
    /// Метод получает тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>Найденный тег, иначе - null.</returns>
    Task<TagEntityModel?> GetTagByIdAsync(int id);

    /// <summary>
    /// Метод получает все теги в формате Dto.
    /// </summary>
    /// <returns>Массив Dto тегов.</returns>
    Task<IEnumerable<TagDto>> GetAllTagsAsync();

    /// <summary>
    /// Метод удаляет тег по его Id.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <returns>True - в случае удачного удаления, false - если тег не был найден.</returns>
    Task<bool> DeleteTagByIdAsync(int id);

    /// <summary>
    /// Метод обновляет информацию о теге.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <param name="name">Новое название.</param>
    /// <param name="description">Новое описание.</param>
    /// <returns>Обновлённую информацию о теге или null если тег не был найден.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<Result<TagEntityModel?>> UpdateTagByIdAsync(int id, string? name, string? description);
}