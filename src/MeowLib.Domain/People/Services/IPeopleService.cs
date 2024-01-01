using MeowLib.Domain.Author.Dto;
using MeowLib.Domain.People.Entity;
using MeowLib.Domain.Shared;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Result;

namespace MeowLib.Domain.Author.Services;

/// <summary>
/// Абстракция сервиса для работы с авторами.
/// </summary>
public interface IPeopleService
{
    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>DTO-модель автора.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<Result<PeopleEntityModel>> CreateAuthorAsync(string name);

    /// <summary>
    /// Метод получает всех авторов.
    /// </summary>
    /// <returns>DTO список авторов.</returns>
    Task<IEnumerable<PeopleDto>> GetAllAuthorsAsync();

    /// <summary>
    /// Метод обновляет информацию об авторе.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <param name="data">Данные для обновления.</param>
    /// <returns>Обновлённую модель данных.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если введёные данные некорректны.</exception>
    Task<Result<PeopleEntityModel?>> UpdateAuthorAsync(int id, PeopleDto data);

    /// <summary>
    /// Метод удаляет автора.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>True - в случае удачного удаления, false - в случае если автор не найден.</returns>
    /// <exception cref="ApiException">Возникает в случае внутренней ошибки.</exception>
    Task<Result<bool>> DeleteAuthorAsync(int id);

    /// <summary>
    /// Метод получает автора по его Id.
    /// </summary>
    /// <param name="authorId">Id автора.</param>
    /// <returns>DTO-модель автора.</returns>
    Task<PeopleEntityModel?> GetAuthorByIdAsync(int authorId);

    /// <summary>
    /// Метод получает список авторов подходящих под поисковые параметры.
    /// </summary>
    /// <param name="name">Имя автора</param>
    /// <returns>Список авторов подходящих под параметры поиска.</returns>
    /// <exception cref="SearchNotFoundException">Возникает если не был найден автор по заданным параметрам поиска.</exception>
    Task<Result<IEnumerable<PeopleDto>>> GetAuthorWithParams(string? name);
}