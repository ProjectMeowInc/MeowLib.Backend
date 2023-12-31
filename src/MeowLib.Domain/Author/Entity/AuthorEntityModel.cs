namespace MeowLib.Domain.Author.Entity;

/// <summary>
/// Класс автора, хранящийся в БД.
/// </summary>
public class AuthorEntityModel
{
    /// <summary>
    /// Id автора.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Имя автора.
    /// </summary>
    public required string Name { get; set; }
}