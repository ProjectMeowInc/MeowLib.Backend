using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
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
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntityModel>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books);

        modelBuilder.Entity<BookEntityModel>()
            .HasMany(b => b.Chapters)
            .WithOne(c => c.Book);

        modelBuilder.Entity<BookEntityModel>()
            .HasOne(b => b.Author)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<BookEntityModel>()
            .HasMany<BookmarkEntityModel>()
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookEntityModel>()
            .HasMany<BookCommentEntityModel>()
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public DbSet<UserEntityModel> Users { get; set; } = null!;
    
    /// <summary>
    /// Таблица авторов.
    /// </summary>
    public DbSet<AuthorEntityModel> Authors { get; set; } = null!;
    
    /// <summary>
    /// Таблица тегов.
    /// </summary>
    public DbSet<TagEntityModel> Tags { get; set; } = null!;

    /// <summary>
    /// Таблица книг.
    /// </summary>
    public DbSet<BookEntityModel> Books { get; set; } = null!;

    /// <summary>
    /// Таблица глав книг.
    /// </summary>
    public DbSet<ChapterEntityModel> Chapters { get; set; } = null!;

    /// <summary>
    /// Таблица избранных книг пользователя.
    /// </summary>
    public DbSet<UserFavoriteEntityModel> UsersFavorite { get; set; } = null!;

    /// <summary>
    /// Таблица закладок пользователей.
    /// </summary>
    public DbSet<BookmarkEntityModel> Bookmarks { get; set; } = null!;

    /// <summary>
    /// Таблциа комментариев к книге.
    /// </summary>
    public DbSet<BookCommentEntityModel> BookComments { get; set; } = null!;
}