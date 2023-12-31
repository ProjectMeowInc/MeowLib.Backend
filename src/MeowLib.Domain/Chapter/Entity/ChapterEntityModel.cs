using MeowLib.Domain.Translation.Entity;

namespace MeowLib.Domain.Chapter.Entity;

/// <summary>
/// Класс главы, хранящийся в БД.
/// </summary>
public class ChapterEntityModel
{
    /// <summary>
    /// Id главы.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Имя главы.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Текст главы.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Позиция главы в переводе.
    /// </summary>
    public required uint Position { get; init; }

    /// <summary>
    /// Время загрузки главы.
    /// </summary>
    public required DateTime ReleaseDate { get; init; }

    /// <summary>
    /// Перевод, к которой принадлежит глава.
    /// </summary>
    public required TranslationEntityModel Translation { get; init; }
}