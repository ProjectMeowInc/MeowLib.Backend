using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.User.Entity;

namespace MeowLib.Domain.Bookmark.Entity;

/// <summary>
/// Класс, описывающий закладку в БД.
/// </summary>
public class BookmarkEntityModel
{
    /// <summary>
    /// Id закладки.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Владелец закладки.
    /// </summary>
    public required UserEntityModel User { get; init; }

    /// <summary>
    /// Глава к которой привязана закладка.
    /// </summary>
    public required ChapterEntityModel Chapter { get; set; }
}