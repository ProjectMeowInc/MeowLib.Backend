using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.Dto.Tag;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;
using MeowLib.Domain.Result;

namespace MeowLIb.WebApi.Services.Interface;

/// <summary>
/// Абстракция сервси для работы с тегами.
/// </summary>
public interface ITagService
{
    /// <summary>
    /// Метод создаёт новый тег.
    /// </summary>
    /// <param name="createTagEntityModel">Данные для создания тега.</param>
    /// <returns>Информацию о созданном теге.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<TagEntityModel>> CreateTagAsync(CreateTagEntityModel createTagEntityModel);

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
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<bool>> DeleteTagByIdAsync(int id);

    /// <summary>
    /// Метод обновляет информацию о теге.
    /// </summary>
    /// <param name="id">Id тега.</param>
    /// <param name="updateTagEntityModel">Данные для обновления.</param>
    /// <returns>Обновлённую информацию о теге или null если тег не был найден.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    /// <exception cref="ApiException">Возникает в случае ошибки сохранения данных.</exception>
    Task<Result<TagEntityModel?>> UpdateTagByIdAsync(int id, UpdateTagEntityModel updateTagEntityModel);
}