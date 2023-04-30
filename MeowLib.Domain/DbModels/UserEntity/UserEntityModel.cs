namespace MeowLib.Domain.DbModels.UserEntity;

/// <summary>
/// Класс пользователя, хранящийся в БД.
/// </summary>
public class UserEntityModel
{
    /// <summary>
    /// Id пользователя. PK.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; } = null!;
    
    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public string Password { get; set; } = null!;
}