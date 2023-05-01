namespace MeowLib.Domain.DbModels.AuthorEntity;

/// <summary>
/// Класс автора, хранящийся в БД.
/// </summary>
public class AuthorEntityModel
{
    /// <summary>
    /// Id автора.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Имя автора.
    /// </summary>
    public string Name { get; set; } = null!;
}