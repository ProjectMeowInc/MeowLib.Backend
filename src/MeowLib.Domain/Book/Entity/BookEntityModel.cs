using MeowLib.Domain.BookPeople.Entity;
using MeowLib.Domain.Character.Entity;
using MeowLib.Domain.File.Entity;
using MeowLib.Domain.Tag.Entity;
using MeowLib.Domain.Translation.Entity;

namespace MeowLib.Domain.Book.Entity;

/// <summary>
/// Класс книги, хранящейся в БД.
/// </summary>
public class BookEntityModel
{
    /// <summary>
    /// Id книги.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Имя книги.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Описание книги.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Изображение книги.
    /// </summary>
    public FileEntityModel? Image { get; set; }

    /// <summary>
    /// Связанный с книгой люди.
    /// </summary>
    public required List<BookPeopleEntityModel> Peoples { get; set; }

    /// <summary>
    /// Список переводов книги.
    /// </summary>
    public required IEnumerable<TranslationEntityModel> Translations { get; init; }

    /// <summary>
    /// Список тегов книги.
    /// </summary>
    public required IEnumerable<TagEntityModel> Tags { get; set; }

    /// <summary>
    /// Список персонажей.
    /// </summary>
    public required List<BookCharacterEntityModel> Characters { get; init; }
}