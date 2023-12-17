using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.Dto.Author;

namespace MeowLib.DAL.Repository.Interfaces;

public interface IAuthorRepository
{
    /// <summary>
    /// Метод создаёт нового автора.
    /// </summary>
    /// <param name="createAuthorData">Данный для создания автора.</param>
    /// <returns>DTO-модель автора</returns>
    Task<AuthorDto> CreateAsync(CreateAuthorEntityModel createAuthorData);

    /// <summary>
    /// Метод получает модель автора по его Id.
    /// </summary>
    /// <param name="id">Id автора</param>
    /// <returns>Модель автора</returns>
    Task<AuthorEntityModel?> GetByIdAsync(int id);

    /// <summary>
    /// Метод удаляет автора по Id.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <returns>True - в случае удачного удаления, False - если автор не найден. </returns>
    /// <exception cref="DbSavingException">Возникает в случае неудачного сохранения данных.</exception>
    Task<bool> DeleteByIdAsync(int id);

    /// <summary>
    /// Метод получает список всех авторов
    /// </summary>
    /// <returns>Список всех авторов</returns>
    IQueryable<AuthorEntityModel> GetAll();

    /// <summary>
    /// Метод обновляет автора по Id.
    /// </summary>
    /// <param name="id">Id автора.</param>
    /// <param name="updateAuthorData">Данные для обновления.</param>
    /// <returns>Обновлённую информацию об авторе.</returns>
    Task<AuthorDto?> UpdateByIdAsync(int id, UpdateAuthorEntityModel updateAuthorData);
}