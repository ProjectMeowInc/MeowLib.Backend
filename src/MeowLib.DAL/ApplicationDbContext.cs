using MeowLib.Domain.Author.Entity;
using MeowLib.Domain.Book.Entity;
using MeowLib.Domain.BookComment.Entity;
using MeowLib.Domain.Bookmark.Entity;
using MeowLib.Domain.Chapter.Entity;
using MeowLib.Domain.CoinsChangeLog.Entity;
using MeowLib.Domain.File.Entity;
using MeowLib.Domain.Notification.Entity;
using MeowLib.Domain.Tag.Entity;
using MeowLib.Domain.Team.Entity;
using MeowLib.Domain.TeamMember.Entity;
using MeowLib.Domain.Translation.Entity;
using MeowLib.Domain.User.Entity;
using MeowLib.Domain.UserFavorite.Entity;
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
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// Таблица пользователей.
    /// </summary>
    public required DbSet<UserEntityModel> Users { get; init; }

    /// <summary>
    /// Таблица авторов.
    /// </summary>
    public required DbSet<AuthorEntityModel> Authors { get; init; }

    /// <summary>
    /// Таблица тегов.
    /// </summary>
    public required DbSet<TagEntityModel> Tags { get; init; }

    /// <summary>
    /// Таблица книг.
    /// </summary>
    public required DbSet<BookEntityModel> Books { get; init; }

    /// <summary>
    /// Таблица глав книг.
    /// </summary>
    public required DbSet<ChapterEntityModel> Chapters { get; init; }

    /// <summary>
    /// Таблица избранных книг пользователя.
    /// </summary>
    public required DbSet<UserFavoriteEntityModel> UsersFavorite { get; init; }

    /// <summary>
    /// Таблица закладок пользователей.
    /// </summary>
    public required DbSet<BookmarkEntityModel> Bookmarks { get; init; }

    /// <summary>
    /// Таблциа комментариев к книге.
    /// </summary>
    public required DbSet<BookCommentEntityModel> BookComments { get; init; }

    /// <summary>
    /// Таблица команд.
    /// </summary>
    public required DbSet<TeamEntityModel> Teams { get; init; }

    /// <summary>
    /// Таблица членов команд.
    /// </summary>
    public required DbSet<TeamMemberEntityModel> TeamMembers { get; init; }

    /// <summary>
    /// Таблица уведомлений пользователей.
    /// </summary>
    public required DbSet<NotificationEntityModel> Notifications { get; init; }

    /// <summary>
    /// Таблица переводов.
    /// </summary>
    public required DbSet<TranslationEntityModel> Translations { get; init; }

    /// <summary>
    /// Таблица логов измененния денег пользователя.
    /// </summary>
    public required DbSet<CoinsChangeLogEntityModel> CoinsChangeLog { get; init; }

    /// <summary>
    /// Таблица файлов.
    /// </summary>
    public required DbSet<FileEntityModel> Files { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookEntityModel>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books);

        modelBuilder.Entity<BookEntityModel>()
            .HasMany(b => b.Translations)
            .WithOne(t => t.Book)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<TranslationEntityModel>()
            .HasMany(t => t.Chapters)
            .WithOne(c => c.Translation)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}