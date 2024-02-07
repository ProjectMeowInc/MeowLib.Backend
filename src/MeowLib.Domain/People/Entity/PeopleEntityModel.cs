using MeowLib.Domain.BookPeople.Entity;

namespace MeowLib.Domain.People.Entity;

/// <summary>
/// Класс человека, хранящийся в БД.
/// </summary>
public class PeopleEntityModel
{
    /// <summary>
    /// Id человека.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Имя человека.
    /// </summary>
    public required string Name { get; set; }

    public required List<BookPeopleEntityModel> BooksPeople { get; init; }
}