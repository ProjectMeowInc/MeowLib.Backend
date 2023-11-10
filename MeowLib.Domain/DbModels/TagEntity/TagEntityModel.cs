using MeowLib.Domain.DbModels.BookEntity;

namespace MeowLib.Domain.DbModels.TagEntity;

/// <summary>
/// Класс описывающий тег.
/// </summary>
public class TagEntityModel
{
    /// <summary>
    /// Id тега.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название тега.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Описание тега.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Список книг с данным тегом.
    /// </summary>
    public IEnumerable<BookEntityModel> Books { get; set; } = null!;
}