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
    public string Name { get; set; } = null!;
    
    /// <summary>
    /// Описание книги.
    /// </summary>
    public string Description { get; set; } = null!;
    
    /// <summary>
    /// Автор книги. FK.
    /// </summary>
    public AuthorEntityModel? Author { get; set; }
    
    /// <summary>
    /// Список глав книги.
    /// </summary>
    public IEnumerable<ChapterEntityModel> Chapters { get; set; } = null!;
    
    /// <summary>
    /// Список тегов книги.
    /// </summary>
    public IEnumerable<TagEntityModel> Tags { get; set; } = null!;
}