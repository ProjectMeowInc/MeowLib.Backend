using MeowLib.Domain.People.Dto;
using MeowLib.Domain.People.Entity;
using MeowLib.Domain.Shared.Exceptions.Services;
using MeowLib.Domain.Shared.Result;

namespace MeowLib.Domain.People.Services;

/// <summary>
/// Абстракция сервиса для работы с авторами.
/// </summary>
public interface IPeopleService
{
    /// <summary>
    /// Метод создаёт нового человека.
    /// </summary>
    /// <param name="name">Имя человека.</param>
    /// <returns>DTO-модель человека.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<Result<PeopleEntityModel>> CreatePeopleAsync(string name);

    /// <summary>
    /// Метод получает всех людей.
    /// </summary>
    /// <returns>DTO список людей.</returns>
    Task<IEnumerable<PeopleDto>> GetAllPeoplesAsync();

    /// <summary>
    /// Метод получает всех людей с разбиением на страницы.
    /// </summary>
    /// <param name="perPageCount">Количество людей на странице.</param>
    /// <param name="page">Номер страницы. Начиная с 0.</param>
    /// <returns>Найденные люди.</returns>
    Task<List<PeopleEntityModel>> GetAllPeoplesWithPageAsync(int perPageCount, int page);

    /// <summary>
    /// Метод обновляет информацию об человеке.
    /// </summary>
    /// <param name="id">Id человека.</param>
    /// <param name="data">Данные для обновления.</param>
    /// <returns>Обновлённую модель человека.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если введёные данные некорректны.</exception>
    Task<Result<PeopleEntityModel?>> UpdatePeopleAsync(int id, PeopleDto data);

    /// <summary>
    /// Метод удаляет человека.
    /// </summary>
    /// <param name="peopleId">Id человека.</param>
    /// <returns>True - в случае удачного удаления, false - в случае если человек не найден.</returns>
    Task<Result<bool>> DeletePeopleAsync(int peopleId);

    /// <summary>
    /// Метод получает человека по его Id.
    /// </summary>
    /// <param name="peopleId">Id автора.</param>
    /// <returns>DTO-модель автора.</returns>
    Task<PeopleEntityModel?> GetPeopleByIdAsync(int peopleId);

    /// <summary>
    /// Метод получает список людей подходящих под поисковые параметры.
    /// </summary>
    /// <param name="name">Имя человека.</param>
    /// <returns>Список людей подходящих под параметры поиска.</returns>
    /// <exception cref="SearchNotFoundException">Возникает если не был найден человек по заданным параметрам поиска.</exception>
    Task<Result<IEnumerable<PeopleDto>>> GetPeopleWithParams(string? name);
}