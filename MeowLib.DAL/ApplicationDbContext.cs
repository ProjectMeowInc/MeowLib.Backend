using MeowLib.Domain.DbModels.AuthorEntity;
using MeowLib.Domain.DbModels.BookCommentEntity;
using MeowLib.Domain.DbModels.BookEntity;
using MeowLib.Domain.DbModels.BookmarkEntity;
using MeowLib.Domain.DbModels.ChapterEntity;
using MeowLib.Domain.DbModels.TagEntity;
using MeowLib.Domain.DbModels.TeamEntity;
using MeowLib.Domain.DbModels.TeamMemberEntity;
using MeowLib.Domain.DbModels.UserEntity;
using MeowLib.Domain.DbModels.UserFavoriteEntity;
using Microsoft.EntityFrameworkCore;

namespace MeowLib.DAL;

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
    public required DbSet<UserEntityModel> Users { get; set; }

    /// <summary>
    /// Таблица авторов.
    /// </summary>
    public required DbSet<AuthorEntityModel> Authors { get; set; }

    /// <summary>
    /// Таблица тегов.
    /// </summary>
    public required DbSet<TagEntityModel> Tags { get; set; }

    /// <summary>
    /// Таблица книг.
    /// </summary>
    public required DbSet<BookEntityModel> Books { get; set; }

    /// <summary>
    /// Таблица глав книг.
    /// </summary>
    public required DbSet<ChapterEntityModel> Chapters { get; set; }

    /// <summary>
    /// Таблица избранных книг пользователя.
    /// </summary>
    public required DbSet<UserFavoriteEntityModel> UsersFavorite { get; set; }

    /// <summary>
    /// Таблица закладок пользователей.
    /// </summary>
    public required DbSet<BookmarkEntityModel> Bookmarks { get; set; }

    /// <summary>
    /// Таблциа комментариев к книге.
    /// </summary>
    public required DbSet<BookCommentEntityModel> BookComments { get; set; }

    /// <summary>
    /// Таблица команд.
    /// </summary>
    public required DbSet<TeamEntityModel> Teams { get; set; }

    /// <summary>
    /// Таблица членов команд.
    /// </summary>
    public required DbSet<TeamMemberEntityModel> TeamMembers { get; set; }
}