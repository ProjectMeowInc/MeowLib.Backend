using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;
using MeowLib.Domain.Exceptions;
using MeowLib.Domain.Exceptions.Services;

namespace MeowLIb.WebApi.Services.Interface;

public interface IAuthorService
{
    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="name">Имя автора.</param>
    /// <returns>DTO-модель автора.</returns>
    /// <exception cref="ValidationException">Возникает в случае ошибки валидации данных.</exception>
    Task<AuthorDto> CreateAuthor(string name);

    /// <summary>
    /// Метод получает всех авторов.
    /// </summary>
    /// <returns>DTO список авторов.</returns>
    Task<IEnumerable<AuthorDto>> GetAllAuthors();

    /// <summary>
    /// Метод обновляет информацию об авторе.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <param name="updateAuthorEntityModel">Данные для обновления.</param>
    /// <returns>Обновлённую модель данных.</returns>
    /// <exception cref="ValidationException">Возникает в случае, если введёные данные некорректны.</exception>
    /// <exception cref="ApiException">Возникает если автор не был найден.</exception>
    Task<AuthorDto> UpdateAuthor(int id, UpdateAuthorEntityModel updateAuthorEntityModel);
}