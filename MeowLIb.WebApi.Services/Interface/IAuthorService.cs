using MeowLib.Domain.Dto.Author;
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
}