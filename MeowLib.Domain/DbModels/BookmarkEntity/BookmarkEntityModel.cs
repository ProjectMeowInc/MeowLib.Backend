using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.UserEntity;

namespace MeowLib.Domain.DbModels.BookmarkEntity;

/// <summary>
/// Класс, описывающий закладку в БД.
/// </summary>
public class BookmarkEntityModel
{
    /// <summary>
    /// Id закладки.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Владелец закладки.
    /// </summary>
    public required UserEntityModel User { get; set; }
    
    /// <summary>
    /// Глава к которой привязана закладка.
    /// </summary>
    public required ChapterEntityModel Chapter { get; set; }
}