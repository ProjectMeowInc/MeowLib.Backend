using MeowLib.Domain.DbModels.TranslationEntity;

namespace MeowLib.Domain.DbModels.ChapterEntity;

/// <summary>
/// Класс главы, хранящийся в БД.
/// </summary>
public class ChapterEntityModel
{
    /// <summary>
    /// Id главы.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя главы.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Текст главы.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Позиция главы в переводе.
    /// </summary>
    public required uint Position { get; set; }
    
    /// <summary>
    /// Время загрузки главы.
    /// </summary>
    public required DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Перевод, к которой принадлежит глава.
    /// </summary>
    public required TranslationEntityModel Translation { get; set; }
}