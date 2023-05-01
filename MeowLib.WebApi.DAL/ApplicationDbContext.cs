using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.UserEntity;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.WebApi.DAL;

/// <summary>
/// Класс для работы EF.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options">Настройки БД.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<UserEntityModel> Users { get; set; } = null!;
    
    /// <summary>
    /// Таблица авторов.
    /// </summary>
    public DbSet<AuthorEntityModel> Authors { get; set; } = null!;
}