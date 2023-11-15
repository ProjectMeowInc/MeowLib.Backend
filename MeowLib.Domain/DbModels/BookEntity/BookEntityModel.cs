using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.TranslationEntity;

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
    public required AuthorEntityModel Author { get; set; }

    /// <summary>
    /// Список переводов книги.
    /// </summary>
    public required IEnumerable<TranslationEntityModel> Translations { get; set; }

    /// <summary>
    /// Список тегов книги.
    /// </summary>
    public required IEnumerable<TagEntityModel> Tags { get; set; }
}