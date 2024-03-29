using MeowLib.Domain.Book.Entity;

namespace MeowLib.Domain.Tag.Entity;

/// <summary>
/// Класс описывающий тег.
/// </summary>
public class TagEntityModel
{
    /// <summary>
    /// Id тега.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Название тега.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Описание тега.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Список книг с данным тегом.
    /// </summary>
    public required IEnumerable<BookEntityModel> Books { get; init; }
}