using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;

namespace MeowLib.Domain.DbModels.BookEntity;

/// <summary>
/// Класс книги, хранящейся в БД.
/// </summary>
public class BookEntityModel
{
    /// <summary>
    /// Id книги.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя книги.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Описание книги.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Название файла обложки книги.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Автор книги. FK.
    /// </summary>
    public AuthorEntityModel? Author { get; set; }

    /// <summary>
    /// Список глав книги.
    /// </summary>
    public IEnumerable<ChapterEntityModel> Chapters { get; set; } = new List<ChapterEntityModel>();

    /// <summary>
    /// Список тегов книги.
    /// </summary>
    public IEnumerable<TagEntityModel> Tags { get; set; } = new List<TagEntityModel>();
}